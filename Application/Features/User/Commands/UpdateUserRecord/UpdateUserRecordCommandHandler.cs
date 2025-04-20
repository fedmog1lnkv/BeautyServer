using Application.Messaging.Command;
using Domain.Entities;
using Domain.Enums;
using Domain.Errors;
using Domain.Repositories.Records;
using Domain.Repositories.Staffs;
using Domain.Repositories.Utils;
using Domain.Repositories.Venues;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Features.User.Commands.UpdateUserRecord;

public sealed class UpdateUserRecordCommandHandler(
    IStaffRepository staffRepository,
    IRecordRepository
        recordRepository,
    IVenueReadOnlyRepository venueReadOnlyRepository,
    INotificationRepository notificationRepository) : ICommandHandler<UpdateUserRecordCommand, Result>
{
    public async Task<Result> Handle(UpdateUserRecordCommand request, CancellationToken cancellationToken)
    {
        var record = await recordRepository.GetRecordById(request.RecordId, cancellationToken);
        if (record is null)
            return Result.Failure(DomainErrors.Record.NotFound);

        if (record.Status == RecordStatus.Completed || record.UserId != request.UserId)
            return Result.Failure(DomainErrors.Record.CannotUpdate);

        var initiatorStaff = await staffRepository.GetByIdAsync(record.StaffId, cancellationToken);
        if (initiatorStaff is null)
            return Result.Failure(DomainErrors.Staff.NotFound(record.StaffId));

        var staff = await staffRepository.GetByIdWithTimeSlotsAsync(record.StaffId, cancellationToken);
        if (staff is null)
            return Result.Failure(DomainErrors.Staff.NotFound(record.StaffId));

        if (initiatorStaff.Id != staff.Id &&
            (initiatorStaff.OrganizationId != staff.OrganizationId ||
             initiatorStaff.Role != StaffRole.Manager))
            return Result.Failure(DomainErrors.Record.CannotUpdate);

        var result = Result.Success();

        if (request.Status is not null)
        {
            var status = (RecordStatus)Enum.Parse(typeof(RecordStatus), request.Status, true);

            if (status == RecordStatus.Discarded)
            {
                result = record.Discard(request.Comment);
                if (result.IsFailure)
                    return result;

                var venue = await venueReadOnlyRepository.GetByIdAsync(record.VenueId, cancellationToken);
                result = RemoveRecordFromIntervals(staff, venue, record);
                if (result.IsFailure)
                    return result;

                if (staff.Settings.FirebaseToken != null)
                    await notificationRepository.SendOrderNotificationAsync(
                        record.Id,
                        staff.Settings.FirebaseToken,
                        "Клиент отменил запись",
                        $"Клиент отменил запись на услугу «{record.Service.Name.Value}», назначенную на {record.StartTimestamp:dd.MM.yyyy в HH:mm}.");
            }
            else
            {
                return Result.Failure(DomainErrors.Record.CannotUpdate);
            }
        }

        if (!string.IsNullOrWhiteSpace(request.Comment))
        {
            result = record.SetComment(request.Comment);
            if (result.IsFailure)
                return result;
        }

        return Result.Success();
    }

    // TODO : move to helper
    private Result RemoveRecordFromIntervals(
        Staff staff,
        Venue venue,
        Record record)
    {
        var recordStartTimeStamp = TimeZoneInfo.ConvertTimeFromUtc(record.StartTimestamp.DateTime, venue.TimeZone);
        var recordEndTimeStamp = TimeZoneInfo.ConvertTimeFromUtc(record.EndTimestamp.DateTime, venue.TimeZone);

        var recordIntervalResult = Interval.Create(
            recordStartTimeStamp.TimeOfDay,
            recordEndTimeStamp.TimeOfDay);
        if (recordIntervalResult.IsFailure)
            return recordIntervalResult;

        var oldStaffTimeSlot =
            staff.TimeSlots.FirstOrDefault(ts => ts.Date == DateOnly.FromDateTime(recordStartTimeStamp));
        if (oldStaffTimeSlot is null)
            return Result.Failure(DomainErrors.TimeSlot.NotFoundByTime);

        var oldStaffIntervals = oldStaffTimeSlot.Intervals.OrderBy(i => i.Start).ToList();

        var updatedIntervalsResult = AddIntervalToTimeSlot(oldStaffIntervals, recordIntervalResult.Value);
        if (updatedIntervalsResult.IsFailure)
            return updatedIntervalsResult;

        var updateStaffTimeSlotResult = staff.UpdateTimeSlotIntervals(
            oldStaffTimeSlot.Id,
            updatedIntervalsResult.Value);
        if (updateStaffTimeSlotResult.IsFailure)
            return updateStaffTimeSlotResult;

        return Result.Success();
    }

    private Result<List<Interval>> AddIntervalToTimeSlot(List<Interval> oldIntervals, Interval recordInterval)
    {
        var updatedIntervals = new List<Interval>();

        if (oldIntervals.Count == 0)
        {
            updatedIntervals.Add(recordInterval);
            return Result.Success(updatedIntervals);
        }

        var isAdded = false;
        foreach (var interval in oldIntervals)
        {
            if (!isAdded && interval.Start > recordInterval.End)
            {
                updatedIntervals.Add(recordInterval);
                isAdded = true;
            }
            updatedIntervals.Add(interval);
        }

        if (!isAdded)
            updatedIntervals.Add(recordInterval);

        updatedIntervals = updatedIntervals.OrderBy(i => i.Start).ToList();

        for (var i = 1; i < updatedIntervals.Count; i++)
        {
            if (updatedIntervals[i - 1].End >= updatedIntervals[i].Start)
            {
                var createIntervalResult = Interval.Create(updatedIntervals[i - 1].Start, updatedIntervals[i].End);
                if (createIntervalResult.IsFailure)
                    return Result.Failure<List<Interval>>(createIntervalResult.Error);

                updatedIntervals.RemoveAt(i);
                updatedIntervals[i - 1] = createIntervalResult.Value;
                i--;
            }
        }

        return updatedIntervals;
    }
}
using Application.Messaging.Command;
using Domain.Enums;
using Domain.Errors;
using Domain.Repositories.Records;
using Domain.Repositories.Staffs;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Features.Staffs.Commands.UpdateTimeSlot;

public sealed class UpdateTimeSlotCommandHandler(
    IStaffRepository staffRepository,
    IRecordReadOnlyRepository recordReadOnlyRepository) : ICommandHandler<UpdateTimeSlotCommand, Result>
{
    public async Task<Result> Handle(UpdateTimeSlotCommand request, CancellationToken cancellationToken)
    {
        var initiatorStaff = await staffRepository.GetByIdAsync(request.InitiatorId, cancellationToken);
        if (initiatorStaff is null)
            return Result.Failure(DomainErrors.Staff.NotFound(request.InitiatorId));

        var staff = await staffRepository.GetByIdWithTimeSlotsAsync(request.StaffId, cancellationToken);
        if (staff is null)
            return Result.Failure(DomainErrors.Staff.NotFound(request.StaffId));

        if (initiatorStaff.Id != staff.Id &&
            (initiatorStaff.OrganizationId != staff.OrganizationId ||
             initiatorStaff.Role != StaffRole.Manager))
            return Result.Failure(DomainErrors.Staff.StaffCannotUpdate);

        var staffTimeSlot = staff.TimeSlots.FirstOrDefault(ts => ts.Id == request.TimeSlotId);

        if (staffTimeSlot is null)
            return Result.Failure(DomainErrors.TimeSlot.NotFound(request.TimeSlotId));

        // Проверка на то, что новые интервалы не пересекаются с records
        var records = await recordReadOnlyRepository.GetRecordsWithVenueByStaffIdAndDate(
            staff.Id,
            staffTimeSlot.Date,
            cancellationToken);

        foreach (var interval in request.Intervals)
        {
            foreach (var record in records.Where(r => r.Status != RecordStatus.Discarded).ToList())
            {
                var existingStart = TimeZoneInfo.ConvertTimeFromUtc(
                    record.StartTimestamp.DateTime,
                    record.Venue.TimeZone);
                var existingEnd = TimeZoneInfo.ConvertTimeFromUtc(
                    record.EndTimestamp.DateTime,
                    record.Venue.TimeZone);

                if (interval.StartTime < existingEnd.TimeOfDay && interval.EndTime > existingStart.TimeOfDay)
                    return Result.Failure(DomainErrors.TimeSlot.Overlap);
            }
        }

        // Обновление интервалов
        var newIntervals = new List<Interval>();

        foreach (var i in request.Intervals)
        {
            var createIntervalResult = Interval.Create(i.StartTime, i.EndTime);
            if (createIntervalResult.IsFailure)
                return createIntervalResult;

            newIntervals.Add(createIntervalResult.Value);
        }
        
        var mergedIntervalsResult = MergeIntervals(newIntervals);
        if (mergedIntervalsResult.IsFailure)
            return mergedIntervalsResult;

        if (mergedIntervalsResult.Value.Count == 0 && records.Count == 0)
        {
            var deleteTimeSlotInterval = staff.DeleteTimeSlot(staffTimeSlot.Id);
            if (deleteTimeSlotInterval.IsFailure)
                return Result.Failure(deleteTimeSlotInterval.Error);
        }
        else
        {
            var updateStaffIntervalsResult = staff.UpdateTimeSlotIntervals(staffTimeSlot.Id, mergedIntervalsResult.Value);
            if (updateStaffIntervalsResult.IsFailure)
                return Result.Failure(updateStaffIntervalsResult.Error);
        }

        return Result.Success();
    }
    
    private Result<List<Interval>> MergeIntervals(List<Interval> oldIntervals)
    {
        var updatedIntervals = oldIntervals.ToList();

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
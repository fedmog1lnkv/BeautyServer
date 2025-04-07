using Application.Messaging.Command;
using Domain.Entities;
using Domain.Enums;
using Domain.Errors;
using Domain.Repositories.Records;
using Domain.Repositories.Staffs;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Features.Staffs.Commands.UpdateStaffRecord;

public sealed class UpdateStaffRecordCommandHandler(
    IStaffRepository staffRepository,
    IRecordRepository
        recordRepository) : ICommandHandler<UpdateStaffRecordCommand, Result>
{
    public async Task<Result> Handle(UpdateStaffRecordCommand request, CancellationToken cancellationToken)
    {
        var record = await recordRepository.GetRecordById(request.RecordId, cancellationToken);
        if (record is null)
            return Result.Failure(DomainErrors.Record.NotFound);

        if (record.Status == RecordStatus.Completed)
            return Result.Failure(DomainErrors.Record.CannotUpdate);

        var initiatorStaff = await staffRepository.GetByIdAsync(request.InitiatorId, cancellationToken);
        if (initiatorStaff is null)
            return Result.Failure(DomainErrors.Staff.NotFound(request.InitiatorId));

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
            result = request.Status switch
            {
                RecordStatus.Approved => record.Approve(request.Comment),
                RecordStatus.Discarded => record.Discard(request.Comment),
                RecordStatus.Completed => record.Completed(request.Comment),
                _ => result
            };

            if (result.IsFailure)
                return result;
        }

        if (!string.IsNullOrWhiteSpace(request.Comment))
        {
            result = record.SetComment(request.Comment);
            if (result.IsFailure)
                return result;
        }

        if (request.StartTimestamp.HasValue || request.EndTimeStamp.HasValue)
        {
            var removeOldRecordResult = RemoveRecordFromIntervals(staff, record);
            if (removeOldRecordResult.IsFailure)
                return removeOldRecordResult;
            
            var startTimeStamp = request.StartTimestamp ?? record.StartTimestamp;
            var endTimeStamp = request.EndTimeStamp ?? record.EndTimestamp;

            if (startTimeStamp.Date != endTimeStamp.Date)
                return Result.Failure(DomainErrors.TimeSlot.NotSameDay);
            
            var startTime = startTimeStamp.TimeOfDay;
            var endTime = endTimeStamp.TimeOfDay;
            
            var timeSlot = staff.TimeSlots.FirstOrDefault(ts => ts.Date == DateOnly.FromDateTime(startTimeStamp));
            if (timeSlot is null)
                return Result.Failure(DomainErrors.TimeSlot.NotFoundByTime);
            
            var isAvailableTime = timeSlot.Intervals.Any(
                interval =>
                    startTime < interval.End && endTime > interval.Start);

            if (!isAvailableTime)
                return Result.Failure(DomainErrors.TimeSlot.IntervalsOverlap);

            var updatedIntervals = new List<Interval>();

            foreach (var interval in timeSlot.Intervals)
            {
                if (startTime <= interval.End && endTime >= interval.Start)
                {
                    if (interval.Start != startTime)
                    {
                        var createIntervalResult = Interval.Create(interval.Start, startTime);
                        if (createIntervalResult.IsFailure)
                            return createIntervalResult;

                        updatedIntervals.Add(createIntervalResult.Value);
                    }

                    if (interval.End != endTime)
                    {
                        var createIntervalResult = Interval.Create(endTime, interval.End);
                        if (createIntervalResult.IsFailure)
                            return createIntervalResult;

                        updatedIntervals.Add(createIntervalResult.Value);
                    }
                }
                else
                {
                    updatedIntervals.Add(interval);
                }
            }

            var updateStaffIntervalsResult = staff.UpdateTimeSlotIntervals(timeSlot.Id, updatedIntervals);
            if (updateStaffIntervalsResult.IsFailure)
                return Result.Failure(updateStaffIntervalsResult.Error);

            result = record.SetTime(startTimeStamp, endTimeStamp);
            if (result.IsFailure)
                return result;
        }

        return Result.Success();
    }

    private Result RemoveRecordFromIntervals(Staff staff, Record record)
    {
        var recordIntervalResult = Interval.Create(record.StartTimestamp.TimeOfDay, record.EndTimestamp.TimeOfDay);
        if (recordIntervalResult.IsFailure)
            return recordIntervalResult;

        var oldStaffTimeSlot =
            staff.TimeSlots.FirstOrDefault(ts => ts.Date == DateOnly.FromDateTime(record.StartTimestamp));
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

        for (var i = 1; i <= updatedIntervals.Count; i++)
        {
            if (updatedIntervals[i - 1].End == updatedIntervals[i].Start)
            {
                var createIntervalResult = Interval.Create(updatedIntervals[i - 1].Start, updatedIntervals[i].End);
                if (createIntervalResult.IsFailure)
                    return Result.Failure<List<Interval>>(createIntervalResult.Error);

                updatedIntervals[i - 1] = createIntervalResult.Value;
                i--;
            }
        }

        return updatedIntervals;
    }
}
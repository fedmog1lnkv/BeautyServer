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

        var intervals = staffTimeSlot.Intervals;

        // Проверка на то, что новые интервалы не пересекаются с records
        var records = await recordReadOnlyRepository.GetRecordsWithVenueByStaffIdAndDate(
            staff.Id,
            staffTimeSlot.Date,
            cancellationToken);

        var baseDate = staffTimeSlot.Date.ToDateTime(TimeOnly.MinValue);
        foreach (var interval in request.Intervals)
        {
            foreach (var record in records)
            {
                var newStart = TimeZoneInfo.ConvertTime(baseDate + interval.StartTime, record.Venue.TimeZone);
                var newEnd = TimeZoneInfo.ConvertTime(baseDate + interval.EndTime, record.Venue.TimeZone);
                
                var existingStart = TimeZoneInfo.ConvertTime(record.StartTimestamp, record.Venue.TimeZone);
                var existingEnd = TimeZoneInfo.ConvertTime(record.EndTimestamp, record.Venue.TimeZone);

                if (newStart < existingEnd && newEnd > existingStart)
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
        
        var updateStaffIntervalsResult = staff.UpdateTimeSlotIntervals(staffTimeSlot.Id, newIntervals);
        if (updateStaffIntervalsResult.IsFailure)
            return Result.Failure(updateStaffIntervalsResult.Error);

        return Result.Success();
    }
}
using Application.Messaging.Command;
using Domain.Entities;
using Domain.Errors;
using Domain.Repositories.Staffs;
using Domain.Repositories.Venues;
using Domain.Shared;
using Domain.ValueObjects;
using System.Runtime.InteropServices.ComTypes;

namespace Application.Features.Staffs.Commands.AddTimeSlot;

public class AddTimeSlotCommandHandler(
    IStaffRepository staffRepository,
    IVenueReadOnlyRepository venueReadOnlyRepository)
    : ICommandHandler<AddTimeSlotCommand, Result>
{
    public async Task<Result> Handle(AddTimeSlotCommand request, CancellationToken cancellationToken)
    {
        var staff = await staffRepository.GetByIdWithTimeSlotsAsync(request.StaffId, cancellationToken);
        if (staff is null)
            return Result.Failure(DomainErrors.Staff.NotFound(request.StaffId));

        var addTimeSlotResult = await staff.AddTimeSlotAsync(
            Guid.NewGuid(),
            request.VenueId,
            request.Date,
            venueReadOnlyRepository,
            cancellationToken);

        if (addTimeSlotResult.IsFailure)
            return addTimeSlotResult;

        var staffTimeSlot = staff.TimeSlots.FirstOrDefault(ts => ts.Date == request.Date);

        if (staffTimeSlot is null)
            return Result.Failure(DomainErrors.TimeSlot.NotFound(Guid.Empty));

        foreach (var intervalDto in request.Intervals)
        {
            var addIntervalResult = staff.AddTimeSlotInterval(
                staffTimeSlot.Id,
                intervalDto.StartTime,
                intervalDto.EndTime);

            if (addIntervalResult.IsFailure)
                return addIntervalResult;
        }

        return Result.Success();
    }
}
using Application.Messaging.Command;
using Domain.Enums;
using Domain.Errors;
using Domain.Repositories.Staffs;
using Domain.Repositories.Venues;
using Domain.Shared;

namespace Application.Features.Staffs.Commands.AddTimeSlot;

public class AddTimeSlotCommandHandler(
    IStaffRepository staffRepository,
    IVenueReadOnlyRepository venueReadOnlyRepository)
    : ICommandHandler<AddTimeSlotCommand, Result>
{
    public async Task<Result> Handle(AddTimeSlotCommand request, CancellationToken cancellationToken)
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
        
        var staffTimeSlot = staff.TimeSlots.FirstOrDefault(ts => ts.Date == request.Date && ts.VenueId == request.VenueId);

        if (staffTimeSlot is not null)
            return Result.Failure(DomainErrors.TimeSlot.AlreadyExists);

        var timeSlotId = Guid.NewGuid();

        var addTimeSlotResult = await staff.AddTimeSlotAsync(
            timeSlotId,
            request.VenueId,
            request.Date,
            venueReadOnlyRepository,
            cancellationToken);

        if (addTimeSlotResult.IsFailure)
            return addTimeSlotResult;

        foreach (var intervalDto in request.Intervals)
        {
            var addIntervalResult = staff.AddTimeSlotInterval(
                timeSlotId,
                intervalDto.StartTime,
                intervalDto.EndTime);

            if (addIntervalResult.IsFailure)
                return addIntervalResult;
        }

        return Result.Success();
    }
}
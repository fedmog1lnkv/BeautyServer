using Application.Messaging.Command;
using Domain.Enums;
using Domain.Errors;
using Domain.Repositories.Staffs;
using Domain.Shared;

namespace Application.Features.Staffs.Commands.DeleteStaff;

public class DeleteStaffCommandHandler(IStaffRepository staffRepository) : ICommandHandler<DeleteStaffCommand, Result>
{
    public async Task<Result> Handle(DeleteStaffCommand request, CancellationToken cancellationToken)
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
            return Result.Failure(DomainErrors.Staff.StaffCannotDelete);

        staffRepository.Remove(staff);

        return Result.Success();
    }
}
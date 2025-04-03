using Application.Messaging.Command;
using Domain.Enums;
using Domain.Errors;
using Domain.Repositories.Staffs;
using Domain.Shared;

namespace Application.Features.Staffs.Commands.UpdateStaff;

public sealed class UpdateStaffCommandHandler(IStaffRepository staffRepository)
    : ICommandHandler<UpdateStaffCommand, Result>
{
    public async Task<Result> Handle(UpdateStaffCommand request, CancellationToken cancellationToken)
    {
        var initiatorStaff = await staffRepository.GetByIdAsync(request.InitiatorId, cancellationToken);
        if (initiatorStaff is null)
            return Result.Failure(DomainErrors.Staff.NotFound(request.InitiatorId));

        var staff = await staffRepository.GetByIdAsync(request.StaffId, cancellationToken);
        if (staff is null)
            return Result.Failure(DomainErrors.Staff.NotFound(request.StaffId));

        if (initiatorStaff.Id != staff.Id && 
            (initiatorStaff.OrganizationId != staff.OrganizationId || 
             initiatorStaff.Role != StaffRole.Manager))
            return Result.Failure(DomainErrors.Staff.StaffCannotUpdate);

        var result = Result.Success();

        if (request.Name != null)
        {
            result = staff.SetName(request.Name);
            if (result.IsFailure)
                return result;
        }

        if (request.Photo != null)
        {
            var photoUrl = await staffRepository.UploadPhotoAsync(request.Photo, staff.Id.ToString());

            if (string.IsNullOrEmpty(photoUrl))
                return Result.Failure(DomainErrors.Staff.PhotoUploadFailed);

            var setPhotoResult = staff.SetPhoto(photoUrl);
            if (setPhotoResult.IsFailure)
                return setPhotoResult;
        }

        return result;
    }
}
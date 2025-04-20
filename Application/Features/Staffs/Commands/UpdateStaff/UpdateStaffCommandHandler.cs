using Application.Messaging.Command;
using Domain.Entities;
using Domain.Enums;
using Domain.Errors;
using Domain.Repositories.Services;
using Domain.Repositories.Staffs;
using Domain.Shared;

namespace Application.Features.Staffs.Commands.UpdateStaff;

public sealed class UpdateStaffCommandHandler(
    IStaffRepository staffRepository,
    IStaffReadOnlyRepository staffReadOnlyRepository,
    IServiceRepository serviceRepository)
    : ICommandHandler<UpdateStaffCommand, Result>
{
    public async Task<Result> Handle(UpdateStaffCommand request, CancellationToken cancellationToken)
    {
        var initiatorStaff = await staffReadOnlyRepository.GetByIdAsync(request.InitiatorId, cancellationToken);
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

        if (request.ServiceIds is not null)
        {
            var availableServices =
                await serviceRepository.GetByOrganizationId(staff.OrganizationId, cancellationToken);
            var serviceDict = availableServices.ToDictionary(s => s.Id);

            var newServices = new List<Service>();
            foreach (var serviceId in request.ServiceIds)
            {
                if (!serviceDict.TryGetValue(serviceId, out var service))
                    return Result.Failure(DomainErrors.Staff.ServiceNotFoundInOrganization(staff.OrganizationId, serviceId));

                newServices.Add(service);
            }

            var setServicesResult = staff.SetServices(newServices);
            if (setServicesResult.IsFailure)
                return setServicesResult;
        }

        if (request.Photo != null)
        {
            var oldPhotoUrl = staff.Photo?.Value;
            var photoId = Guid.NewGuid();
            var photoUrl = await staffRepository.UploadPhotoAsync(request.Photo, photoId.ToString());

            if (string.IsNullOrEmpty(photoUrl))
                return Result.Failure(DomainErrors.Staff.PhotoUploadFailed);

            var setPhotoResult = staff.SetPhoto(photoUrl);
            if (setPhotoResult.IsFailure)
                return setPhotoResult;
            
            if (oldPhotoUrl is not null)
                await staffRepository.DeletePhoto(oldPhotoUrl);
        }
        
        if (request.FirebaseToken != null)
        {
            result = staff.SetFirebaseToken(request.FirebaseToken);
            if (result.IsFailure)
                return result;
        }

        return result;
    }
}
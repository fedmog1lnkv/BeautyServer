using Application.Messaging.Command;
using Domain.Entities;
using Domain.Errors;
using Domain.Repositories.Services;
using Domain.Repositories.Staffs;
using Domain.Repositories.Venues;
using Domain.Shared;

namespace Application.Features.Services.Commands.UpdateService;

public sealed class UpdateServiceCommandHandler(IServiceRepository serviceRepository, IVenueRepository 
        venueRepository, IStaffRepository staffRepository)
    : ICommandHandler<UpdateServiceCommand, Result>
{
    public async Task<Result> Handle(UpdateServiceCommand request, CancellationToken cancellationToken)
    {
        var service = await serviceRepository.GetById(request.Id, cancellationToken);

        if (service is null)
            return Result.Failure(DomainErrors.Service.NotFound(request.Id));

        var result = Result.Success();

        if (request.Name != null)
        {
            result = service.SetName(request.Name);
            if (result.IsFailure)
                return result;
        }

        if (request.Description != null)
        {
            result = service.SetDescription(request.Description);
            if (result.IsFailure)
                return result;
        }

        if (request.Duration.HasValue)
        {
            result = service.SetDuration(request.Duration.Value);
            if (result.IsFailure)
                return result;
        }

        if (request.Price.HasValue)
        {
            result = service.SetPrice(request.Price.Value);
            if (result.IsFailure)
                return result;
        }
        
        if (request.VenueIds is not null)
        {
            var availableVenues =
                await venueRepository.GetByOrganizationId(service.OrganizationId, cancellationToken);
            var venuesDict = availableVenues.ToDictionary(s => s.Id);

            var newVenues = new List<Venue>();
            foreach (var venueId in request.VenueIds)
            {
                if (!venuesDict.TryGetValue(venueId, out var venue))
                    return Result.Failure(DomainErrors.Service.VenueNotFoundInOrganization(service.OrganizationId, venueId));

                newVenues.Add(venue);
            }

            var setVenuesResult = service.SetVenue(newVenues);
            if (setVenuesResult.IsFailure)
                return setVenuesResult;
        }
        
        if (request.StaffIds is not null)
        {
            var availableStaffs =
                await staffRepository.GetByOrganizationId(service.OrganizationId, cancellationToken);
            var staffsDict = availableStaffs.ToDictionary(s => s.Id);

            var newStaffs = new List<Staff>();
            foreach (var staffId in request.StaffIds)
            {
                if (!staffsDict.TryGetValue(staffId, out var staff))
                    return Result.Failure(DomainErrors.Service.VenueNotFoundInOrganization(service.OrganizationId, staffId));

                newStaffs.Add(staff);
            }

            var setStaffResult = service.SetStaff(newStaffs);
            if (setStaffResult.IsFailure)
                return setStaffResult;
        }
        
        if (request.Photo != null)
        {
            var oldPhotoUrl = service.Photo?.Value;
            var photoId = Guid.NewGuid();
            var photoUrl = await serviceRepository.UploadPhotoAsync(request.Photo, photoId.ToString());

            if (string.IsNullOrEmpty(photoUrl))
                return Result.Failure(DomainErrors.Staff.PhotoUploadFailed);

            var setPhotoResult = service.SetPhoto(photoUrl);
            if (setPhotoResult.IsFailure)
                return setPhotoResult;
            
            if (oldPhotoUrl is not null)
                await serviceRepository.DeletePhoto(oldPhotoUrl);
        }

        return result;
    }
}
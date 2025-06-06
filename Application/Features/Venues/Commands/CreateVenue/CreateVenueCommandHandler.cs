using Application.Messaging.Command;
using Domain.Entities;
using Domain.Errors;
using Domain.Repositories.Organizations;
using Domain.Repositories.Utils;
using Domain.Repositories.Venues;
using Domain.Shared;

namespace Application.Features.Venues.Commands.CreateVenue;

public class CreateVenueCommandHandler(
    IVenueRepository venueRepository,
    IOrganizationReadOnlyRepository organizationReadOnlyRepository,
    ILocationRepository locationRepository)
    : ICommandHandler<CreateVenueCommand, Result>
{
    public async Task<Result> Handle(CreateVenueCommand request, CancellationToken cancellationToken)
    {
        var organization = await organizationReadOnlyRepository.GetByIdAsync(request.OrganizationId, cancellationToken);

        if (organization is null)
            return Result.Failure(DomainErrors.Organization.NotFound(request.OrganizationId));

        var venueColor = request.Color ?? organization.Theme.Color;

        var createVenueResult = await Venue.CreateAsync(
            Guid.NewGuid(),
            organization.Id,
            request.Name,
            request.Address,
            venueColor,
            request.Latitude,
            request.Longitude,
            DateTime.UtcNow,
            organizationReadOnlyRepository,
            locationRepository);

        if (createVenueResult.IsFailure)
            return Result.Failure(createVenueResult.Error);

        var venue = createVenueResult.Value;

        if (request.Description is not null)
        {
            var result = venue.SetDescription(request.Description);
            if (result.IsFailure)
                return Result.Failure(result.Error);
        }
        
        if (request.Photo is not null)
        {
            var isBase64 = Utils.IsBase64String(request.Photo);

            var oldPhotoUrl = venue.Theme.Photo;
            var photoId = Guid.NewGuid();

            var photoUrl = isBase64
                ? await venueRepository.UploadPhotoAsync(request.Photo, photoId.ToString())
                : request.Photo;

            if (string.IsNullOrEmpty(photoUrl))
                return Result.Failure(DomainErrors.Organization.PhotoUploadFailed);

            var result = venue.SetPhoto(photoUrl);
            if (result.IsFailure)
                return Result.Failure(result.Error);
            
            if (oldPhotoUrl is not null)
                await venueRepository.DeletePhoto(oldPhotoUrl);
        }
        else
        {
            var result = venue.SetPhoto(organization.Theme.Photo);
            if (result.IsFailure)
                return Result.Failure(result.Error);
        }

        venueRepository.Add(createVenueResult.Value);

        return Result.Success();
    }
}
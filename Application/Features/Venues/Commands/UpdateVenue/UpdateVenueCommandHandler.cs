using Application.Messaging.Command;
using Domain.Entities;
using Domain.Errors;
using Domain.Repositories.Services;
using Domain.Repositories.Venues;
using Domain.Shared;

namespace Application.Features.Venues.Commands.UpdateVenue;

public class UpdateVenueCommandHandler(
    IVenueRepository venueRepository,
    IServiceReadOnlyRepository serviceReadOnlyRepository) : ICommandHandler<UpdateVenueCommand,
    Result>
{
    public async Task<Result> Handle(UpdateVenueCommand request, CancellationToken cancellationToken)
    {
        var venue = await venueRepository.GetByIdWithServicesAsync(request.Id, cancellationToken);
        if (venue is null)
            return Result.Failure(DomainErrors.Venue.NotFound(request.Id));

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var nameResult = venue.SetName(request.Name);
            if (nameResult.IsFailure)
                return nameResult;
        }

        if (!string.IsNullOrWhiteSpace(request.Description))
        {
            var descResult = venue.SetDescription(request.Description);
            if (descResult.IsFailure)
                return descResult;
        }

        if (!string.IsNullOrWhiteSpace(request.Color))
        {
            var colorResult = venue.SetColor(request.Color);
            if (colorResult.IsFailure)
                return colorResult;
        }


        if (!string.IsNullOrWhiteSpace(request.Photo))
        {
            var isBase64 = request.Photo.Length % 4 == 0 &&
                           request.Photo.Replace(" ", "").Replace("\n", "").Replace("\r", "").All(
                               c => char.IsLetterOrDigit(c) || c == '+' || c == '/' || c == '=');

            var photoUrl = isBase64
                ? await venueRepository.UploadPhotoAsync(request.Photo, venue.Id.ToString())
                : request.Photo;

            if (string.IsNullOrEmpty(photoUrl))
                return Result.Failure(DomainErrors.Organization.PhotoUploadFailed);

            var photoResult = venue.SetPhoto(photoUrl);
            if (photoResult.IsFailure)
                return photoResult;
        }
        if (request.Latitude.HasValue && request.Longitude.HasValue)
        {
            var locationResult = venue.SetLocation(request.Latitude.Value, request.Longitude.Value);
            if (locationResult.IsFailure)
                return locationResult;
        }

        if (request.ServiceIds is not null)
        {
            var availableServices =
                await serviceReadOnlyRepository.GetByOrganizationIdAsync(venue.OrganizationId, cancellationToken);
            var serviceDict = availableServices.ToDictionary(s => s.Id);

            var newServices = new List<Service>();
            foreach (var serviceId in request.ServiceIds)
            {
                if (!serviceDict.TryGetValue(serviceId, out var service))
                    return Result.Failure(DomainErrors.Venue.ServiceNotFoundInOrganization(venue.Id, serviceId));

                newServices.Add(service);
            }

            var setServicesResult = venue.SetServices(newServices);
            if (setServicesResult.IsFailure)
                return setServicesResult;
        }

        return Result.Success();
    }
}
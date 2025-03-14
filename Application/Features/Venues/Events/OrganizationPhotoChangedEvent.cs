using Application.Abstractions;
using Domain.DomainEvents.Organizations;
using Domain.Repositories.Venues;
using Serilog;

namespace Application.Features.Venues.Events;

public class OrganizationPhotoChangedEventHandler(IVenueRepository venueRepository)
    : IDomainEventHandler<OrganizationPhotoChangedEvent>
{
    public async Task Handle(OrganizationPhotoChangedEvent notification, CancellationToken cancellationToken)
    {
        var venues = await venueRepository.GetByOrganizationId(notification.OrganizationId, cancellationToken);

        foreach (var venue in venues)
        {
            if (venue.Theme.Photo is not null)
                if (venue.Theme.Color != notification.OrganizationPhotoOld)
                    continue;

            var setVenuePhotoResult = venue.SetPhoto(notification.OrganizationPhotoNew);
            if (setVenuePhotoResult.IsFailure)
                Log.Error($"Notification failure {nameof(OrganizationPhotoChangedEvent)}");
        }
    }
}
using Application.Abstractions;
using Domain.DomainEvents.Organizations;
using Domain.Repositories.Venues;
using Serilog;

namespace Application.Features.Venues.Events;

public class OrganizationColorChangedEventHandler(IVenueRepository venueRepository)
    : IDomainEventHandler<OrganizationColorChangedEvent>
{
    public async Task Handle(OrganizationColorChangedEvent notification, CancellationToken cancellationToken)
    {
        var venues = await venueRepository.GetByOrganizationId(notification.OrganizationId, cancellationToken);

        foreach (var venue in venues)
        {
            if (venue.Theme.Color != notification.OrganizationColorOld)
                continue;

            var setVenueColorResult = venue.SetColor(notification.OrganizationColorNew);
            if (setVenueColorResult.IsFailure)
                Log.Error($"Notification failure {nameof(OrganizationColorChangedEvent)}");
        }
    }
}
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
            {
                Log.Information($"Skipping venue {venue.Id} because current color does not match the old color.");
                continue;
            }

            var setVenueColorResult = venue.SetColor(notification.OrganizationColorNew);
        
            if (setVenueColorResult.IsFailure)
            {
                Log.Error($"Notification failure {nameof(OrganizationColorChangedEvent)} for venue {venue.Id}. Old color: {notification.OrganizationColorOld}, New color: {notification.OrganizationColorNew}");
            }
            else
            {
                Log.Information($"Successfully updated color for venue {venue.Id}. New color: {notification.OrganizationColorNew}");
            }
        }
    }
}
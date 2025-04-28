using Application.Abstractions;
using Domain.DomainEvents.Organizations;
using Domain.DomainEvents.Record;
using Domain.Repositories;
using Domain.Repositories.Records;
using Domain.Repositories.Venues;
using Serilog;

namespace Application.Features.Venues.Events;

public class RecordReviewAddedChangedEventHandler(
    IVenueRepository venueRepository,
    IRecordRepository recordRepository)
    : IDomainEventHandler<RecordReviewAddedChangedEvent>
{
    public async Task Handle(RecordReviewAddedChangedEvent notification, CancellationToken cancellationToken)
    {
        var venue = await venueRepository.GetByIdAsync(notification.VenueId, cancellationToken);
        if (venue is null)
        {
            Log.Error(
                $"Notification failure {nameof(OrganizationColorChangedEvent)} for venue {notification.ServiceId}. Venue not found.");
            return;
        }

        var records = await recordRepository.GetByVenueId(
            venue.Id,
            int.MaxValue,
            0,
            cancellationToken);

        var recordsWithReviews = records.Where(r => r.Review != null).ToList();
        var avgRating = 0.0;

        if (recordsWithReviews.Any())
            avgRating = recordsWithReviews.Average(r => r.Review?.Rating ?? 0);

        avgRating = Math.Round((avgRating + notification.Rating) / 2, 2);

        var result = venue.SetRating(avgRating);
        if (result.IsFailure)
            Log.Error($"Failed to set rating for venue {venue.Id}. Error: {result.Error}");
    }
}
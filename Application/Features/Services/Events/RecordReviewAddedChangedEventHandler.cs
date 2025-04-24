using Application.Abstractions;
using Domain.DomainEvents.Organizations;
using Domain.DomainEvents.Record;
using Domain.Repositories.Records;
using Domain.Repositories.Services;
using Serilog;

namespace Application.Features.Services.Events;

public class RecordReviewAddedChangedEventHandler(
    IServiceRepository serviceRepository,
    IRecordRepository recordRepository)
    : IDomainEventHandler<RecordReviewAddedChangedEvent>
{
    public async Task Handle(RecordReviewAddedChangedEvent notification, CancellationToken cancellationToken)
    {
        var service = await serviceRepository.GetById(notification.ServiceId, cancellationToken);
        if (service is null)
        {
            Log.Error(
                $"Notification failure {nameof(OrganizationColorChangedEvent)} for service {notification.ServiceId}. Service not found.");
            return;
        }

        var records = await recordRepository.GetByServiceId(
            service.Id,
            int.MaxValue,
            0,
            cancellationToken);

        var avgRating = records
            .Where(r => r.Review != null)
            .Average(r => r.Review?.Rating ?? 0);

        avgRating = Math.Round((avgRating + notification.Rating) / 2, 2);

        var result = service.SetRating(avgRating);
        if (result.IsFailure)
            Log.Error($"Failed to set rating for service {service.Id}. Error: {result.Error}");
    }
}
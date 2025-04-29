using Application.Abstractions;
using Domain.IntegrationEvents.Record;

namespace Application.Features.Records.IntegrationEvents;

internal sealed class RecordStatusLogEventHandler(IHubClient<RecordStatusLogEvent> client)
    : IIntegrationEventHandler<RecordStatusLogEvent>
{
    public async Task Handle(
        RecordStatusLogEvent notification,
        CancellationToken cancellationToken) =>
        await client.SendToRecordGroupAsync(notification.RecordId, notification, cancellationToken);
}
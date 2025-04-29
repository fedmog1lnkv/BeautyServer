using Application.Abstractions;
using Domain.IntegrationEvents.Record;

namespace Application.Features.Records.IntegrationEvents;

internal sealed class RecordReadMessageEventHandler(IHubClient<RecordReadMessageEvent> client)
    : IIntegrationEventHandler<RecordReadMessageEvent>
{
    public async Task Handle(
        RecordReadMessageEvent notification,
        CancellationToken cancellationToken) =>
        await client.SendToRecordGroupAsync(notification.RecordId, notification, cancellationToken);
}
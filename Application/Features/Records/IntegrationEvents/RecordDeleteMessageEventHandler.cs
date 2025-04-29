using Application.Abstractions;
using Domain.IntegrationEvents.Record;

namespace Application.Features.Records.IntegrationEvents;

internal sealed class RecordDeleteMessageEventHandler(IHubClient<RecordDeleteMessageEvent> client)
    : IIntegrationEventHandler<RecordDeleteMessageEvent>
{
    public async Task Handle(
        RecordDeleteMessageEvent notification,
        CancellationToken cancellationToken) =>
        await client.SendToRecordGroupAsync(notification.RecordId, notification, cancellationToken);
}
using Application.Abstractions;
using Domain.IntegrationEvents.Record;

namespace Application.Features.Records.IntegrationEvents;

internal sealed class RecordAddMessageEventHandler(IHubClient<RecordAddMessageEvent> client)
    : IIntegrationEventHandler<RecordAddMessageEvent>
{
    public async Task Handle(
        RecordAddMessageEvent notification,
        CancellationToken cancellationToken) =>
        await client.SendToRecordGroupAsync(notification.RecordId, notification, cancellationToken);
}
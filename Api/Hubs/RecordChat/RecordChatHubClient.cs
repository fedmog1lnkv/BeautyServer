using Domain.Primitives;
using Microsoft.AspNetCore.SignalR;

namespace Api.Hubs.RecordChat;

public sealed class RecordChatHubClient<TIntegrationEvent>(IHubContext<RecordChatHub> hubContext)
    : HubClientBase<RecordChatHub, TIntegrationEvent>(hubContext)
    where TIntegrationEvent : IIntegrationEvent
{
    private readonly IHubContext<RecordChatHub> _hubContext = hubContext;

    public override Task SendToRecordGroupAsync(
        Guid recordId,
        TIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        var groupName = $"Record_{recordId}";
        return _hubContext.Clients.Group(groupName)
            .SendAsync($"Receive{typeof(TIntegrationEvent).Name}", integrationEvent, cancellationToken);
    }
}
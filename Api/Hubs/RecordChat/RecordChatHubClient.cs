using Domain.Primitives;
using Microsoft.AspNetCore.SignalR;

namespace Api.Hubs.RecordChat;

public sealed class RecordChatHubClient<TIntegrationEvent>
    : HubClientBase<RecordChatHub, TIntegrationEvent>
    where TIntegrationEvent : IIntegrationEvent
{
    public RecordChatHubClient(IHubContext<RecordChatHub> hubContext)
        : base(hubContext)
    {
    }
}
using Application.Abstractions;
using Domain.Primitives;
using Microsoft.AspNetCore.SignalR;

namespace Api.Hubs;

public abstract class HubClientBase<THub, TIntegrationEvent>(IHubContext<THub> hubContext)
    : IHubClient<TIntegrationEvent>
    where THub : Hub
    where TIntegrationEvent : IIntegrationEvent
{
    public async Task SendEventAsync(
        TIntegrationEvent integrationEvent,
        CancellationToken cancellationToken) =>
        await hubContext.Clients.All.SendAsync(
            $"Receive{typeof(TIntegrationEvent).Name}",
            integrationEvent,
            cancellationToken);
}
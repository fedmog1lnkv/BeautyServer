using Domain.Primitives;

namespace Application.Abstractions;

public interface IHubClient<TIntegrationEvent>
    where TIntegrationEvent : IIntegrationEvent
{
    Task SendEventAsync(
        TIntegrationEvent integrationEvent,
        CancellationToken cancellationToken);
}
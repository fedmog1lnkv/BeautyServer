using Application.Abstractions;
using Domain.Primitives;

namespace Infrastructure;

public class IntegrationEventBus(InMemoryIntegrationEventsQueue queue) : IIntegrationEventBus
{
    public async Task SendAsync<T>(T integrationEvent, CancellationToken cancellationToken = default)
        where T : class, IIntegrationEvent
    {
        await queue.Writer.WriteAsync(integrationEvent, cancellationToken);
    }
}
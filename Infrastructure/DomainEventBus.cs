using Application.Abstractions;
using Domain.Primitives;

namespace Infrastructure;

public class DomainEventBus(InMemoryDomainEventsQueue queue) : IDomainEventBus
{
    public async Task SendAsync<T>(T domainEvent, CancellationToken cancellationToken = default)
        where T : class, IDomainEvent
    {
        await queue.Writer.WriteAsync(domainEvent, cancellationToken);
    }
}
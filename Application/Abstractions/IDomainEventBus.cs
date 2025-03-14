using Domain.Primitives;

namespace Application.Abstractions;

public interface IDomainEventBus
{
    Task SendAsync<T>(
        T integrationEvent,
        CancellationToken cancellationToken = default)
        where T : class, IDomainEvent;
}
using Domain.Primitives;

namespace Application.Abstractions;

public interface IIntegrationEventBus
{
    Task SendAsync<T>(
        T integrationEvent,
        CancellationToken cancellationToken = default)
        where T : class, IIntegrationEvent;
}
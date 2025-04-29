using Domain.Primitives;

namespace Application.Abstractions;

public interface IHubClient<TIntegrationEvent>
    where TIntegrationEvent : IIntegrationEvent
{
    Task SendEventAsync(
        TIntegrationEvent integrationEvent,
        CancellationToken cancellationToken);

    Task SendToRecordGroupAsync(
        Guid recordId,
        TIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default);
}
using Domain.Primitives;

namespace Domain.IntegrationEvents;

public abstract record IntegrationEvent(Guid Id) : IIntegrationEvent;
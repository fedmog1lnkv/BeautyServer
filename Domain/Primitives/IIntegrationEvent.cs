using MediatR;

namespace Domain.Primitives;

public interface IIntegrationEvent : INotification
{
    Guid Id { get; init; }
}
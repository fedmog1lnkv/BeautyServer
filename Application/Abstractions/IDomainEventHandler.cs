using Domain.Primitives;
using MediatR;

namespace Application.Abstractions;

public interface IDomainEventHandler<TEvent>
    : INotificationHandler<TEvent>
    where TEvent : IDomainEvent
{
}
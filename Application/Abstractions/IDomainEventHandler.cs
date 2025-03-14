using Domain.Primitives;
using MediatR;

namespace Application.Abstractions;

public interface IDomainEventHandler<in TEvent>
    : INotificationHandler<TEvent>
    where TEvent : IDomainEvent
{
}
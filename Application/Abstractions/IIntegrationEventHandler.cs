using Domain.Primitives;
using MediatR;

namespace Application.Abstractions;

public interface IIntegrationEventHandler<TEvent>
    : INotificationHandler<TEvent>
    where TEvent : IIntegrationEvent { }
using MediatR;

namespace Application.Messaging.Command;

public interface ICommand : IRequest { }

public interface ICommand<TResponse> : IRequest<TResponse> { }
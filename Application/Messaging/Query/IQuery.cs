using MediatR;

namespace Application.Messaging.Query;

public interface IQuery<TResponse> : IRequest<TResponse> { }
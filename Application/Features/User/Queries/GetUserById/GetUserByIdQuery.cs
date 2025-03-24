using Application.Messaging.Query;
using Domain.Shared;

namespace Application.Features.User.Queries.GetUserById;

public record GetUserByIdQuery(Guid Id) : IQuery<Result<Domain.Entities.User>>;
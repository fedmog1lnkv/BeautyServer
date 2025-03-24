using Application.Messaging.Query;
using Domain.Errors;
using Domain.Repositories.Users;
using Domain.Shared;

namespace Application.Features.User.Queries.GetUserById;

public class GetUserByIdQueryHandler(IUserReadOnlyRepository repository)
    : IQueryHandler<GetUserByIdQuery,
        Result<Domain.Entities.User>>
{
    public async Task<Result<Domain.Entities.User>> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        var user = await repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (user is null)
            return Result.Failure<Domain.Entities.User>(DomainErrors.User.NotFound(request.Id));

        return Result.Success(user);
    }
}
using Application.Messaging.Query;
using Domain.Entities;
using Domain.Errors;
using Domain.Repositories.Services;
using Domain.Shared;

namespace Application.Features.Services.Queries.GetServiceById;

public class GetServiceByIdQueryHandler(IServiceReadOnlyRepository repository)
    : IQueryHandler<GetServiceByIdQuery,
        Result<Service>>
{
    public async Task<Result<Service>> Handle(GetServiceByIdQuery request, CancellationToken cancellationToken)
    {
        var service = await repository.GetById(
            request.Id,
            cancellationToken);

        if (service is null)
            return Result.Failure<Service>(DomainErrors.Service.NotFound(request.Id));

        return Result.Success(service);
    }
}
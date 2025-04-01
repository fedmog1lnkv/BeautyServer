using Application.Messaging.Query;
using Domain.Entities;
using Domain.Errors;
using Domain.Repositories.Organizations;
using Domain.Shared;

namespace Application.Features.Organizations.Queries.GetOrganiazationById;

public class GetOrganizationByIdQueryHandler(IOrganizationReadOnlyRepository repository)
    : IQueryHandler<GetOrganizationByIdQuery,
        Result<Organization>>
{
    public async Task<Result<Organization>> Handle(
        GetOrganizationByIdQuery request,
        CancellationToken cancellationToken)
    {
        var organization = await repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (organization is null)
            return Result.Failure<Organization>(DomainErrors.Organization.NotFound(request.Id));

        return organization;
    }
}
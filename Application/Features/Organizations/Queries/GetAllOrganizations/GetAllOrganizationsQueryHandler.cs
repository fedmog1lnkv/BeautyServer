using Application.Features.Venues.Queries.GetAllVenues;
using Application.Messaging.Query;
using Domain.Entities;
using Domain.Repositories.Organizations;
using Domain.Repositories.Venues;
using Domain.Shared;

namespace Application.Features.Organizations.Queries.GetAllOrganizations;

public class GetAllOrganizationsQueryHandler(IOrganizationReadOnlyRepository repository) : IQueryHandler<GetAllOrganizationsQuery,
    Result<List<Organization>>>
{
    public async Task<Result<List<Organization>>> Handle(GetAllOrganizationsQuery request, CancellationToken cancellationToken)
    {
        var organizations = await repository.GetAll(request.Limit, request.Offset, cancellationToken);
        
        return Result.Success(organizations);
    }
}
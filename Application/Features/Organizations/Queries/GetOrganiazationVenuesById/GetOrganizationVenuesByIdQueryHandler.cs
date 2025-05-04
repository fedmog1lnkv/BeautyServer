using Application.Messaging.Query;
using Domain.Entities;
using Domain.Repositories.Venues;
using Domain.Shared;

namespace Application.Features.Organizations.Queries.GetOrganiazationVenuesById;

public class GetOrganizationVenuesByIdQueryHandler(IVenueRepository repository)
    : IQueryHandler<GetOrganizationVenuesByIdQuery,
        Result<List<Venue>>>
{
    public async Task<Result<List<Venue>>> Handle(
        GetOrganizationVenuesByIdQuery request,
        CancellationToken cancellationToken)
    {
        var venues = await repository.GetByOrganizationId(
            request.Id,
            cancellationToken);

        return venues;
    }
}
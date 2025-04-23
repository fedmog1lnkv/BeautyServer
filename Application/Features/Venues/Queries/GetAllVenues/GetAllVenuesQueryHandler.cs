using Application.Messaging.Query;
using Domain.Entities;
using Domain.Repositories.Venues;
using Domain.Shared;

namespace Application.Features.Venues.Queries.GetAllVenues;

public class GetAllVenuesQueryHandler(IVenueReadOnlyRepository repository) : IQueryHandler<GetAllVenuesQuery,
    Result<List<Venue>>>
{
    public async Task<Result<List<Venue>>> Handle(GetAllVenuesQuery request, CancellationToken cancellationToken)
    {
        List<Venue> venues;
        if (!request.Latitude.HasValue || !request.Longitude.HasValue)
        {
            venues = await repository.GetAll(request.Limit, request.Offset, request.Search, cancellationToken);
        }
        else
        {
            venues = await repository.GetByLocation(
                (double)request.Latitude!,
                (double)request.Longitude!,
                request.Limit,
                request.Offset,
                request.Search,
                cancellationToken);
        }

        return Result.Success(venues);
    }
}
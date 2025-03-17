using Application.Messaging.Query;
using Domain.Entities;
using Domain.Errors;
using Domain.Repositories.Venues;
using Domain.Shared;

namespace Application.Features.Services.Queries.GetServicesByVenueId;

public class GetServicesByVenueIdQueryHandler(IVenueReadOnlyRepository repository)
    : IQueryHandler<GetServicesByVenueIdQuery,
        Result<List<Service>>>
{
    public async Task<Result<List<Service>>> Handle(
        GetServicesByVenueIdQuery request,
        CancellationToken cancellationToken)
    {
        var venue = await repository.GetByIdWithServicesAsync(
            request.VenueId,
            cancellationToken);

        if (venue is null)
            return Result.Failure<List<Service>>(DomainErrors.Venue.NotFound(request.VenueId));

        return Result.Success(venue.Services.ToList());
    }
}
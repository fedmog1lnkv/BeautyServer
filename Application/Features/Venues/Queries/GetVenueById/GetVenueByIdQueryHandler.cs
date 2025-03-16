using Application.Features.Venues.Queries.GetAllVenues;
using Application.Messaging.Query;
using Domain.Entities;
using Domain.Errors;
using Domain.Repositories.Venues;
using Domain.Shared;

namespace Application.Features.Venues.Queries.GetVenueById;

public class GetVenueByIdQueryHandler(IVenueReadOnlyRepository repository) : IQueryHandler<GetVenueByIdQuery,
    Result<Venue>>
{
    public async Task<Result<Venue>> Handle(GetVenueByIdQuery request, CancellationToken cancellationToken)
    {
        var venue = await repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (venue is null)
            return Result.Failure<Venue>(
                DomainErrors.Venue.NotFound(request.Id));

        return venue;
    }
}
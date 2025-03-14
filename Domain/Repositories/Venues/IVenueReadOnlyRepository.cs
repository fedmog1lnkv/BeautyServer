using Domain.Entities;
using Domain.Repositories.Base;

namespace Domain.Repositories.Venues;

public interface IVenueReadOnlyRepository : IReadOnlyRepository<Venue>
{
    Task<bool> ExistsAsync(
        Guid venueId,
        CancellationToken cancellationToken = default);
}
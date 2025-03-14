using Domain.Entities;
using Domain.Repositories.Venues;

namespace Infrastructure.Repositories.Venues;

public class VenueReadOnlyRepository : IVenueReadOnlyRepository
{
    public async Task<bool> ExistsAsync(Guid venueId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Venue>> GetByLocation(
        double latitude,
        double longitude,
        int limit,
        int offset,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Venue>> GetAll(
        int limit,
        int offset,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
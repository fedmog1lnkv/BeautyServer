using Domain.Repositories.Venues;

namespace Infrastructure.Repositories.Venues;

public class VenueReadOnlyRepository : IVenueReadOnlyRepository
{
    public async Task<bool> ExistsAsync(Guid venueId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
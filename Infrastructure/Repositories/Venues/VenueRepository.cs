using Domain.Entities;
using Domain.Repositories.Venues;

namespace Infrastructure.Repositories.Venues;

public class VenueRepository(ApplicationDbContext dbContext) : IVenueRepository
{
    public async Task<List<Venue>> GetByOrganizationId(Guid organizationId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void Add(Venue venue)
    {
        dbContext.Set<Venue>().Add(venue);
        dbContext.SaveChanges();
    }
}
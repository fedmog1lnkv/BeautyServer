using Domain.Entities;
using Domain.Repositories.Venues;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Venues;

public class VenueRepository(ApplicationDbContext dbContext) : IVenueRepository
{
    public async Task<List<Venue>> GetByOrganizationId(
        Guid organizationId,
        CancellationToken cancellationToken = default) =>
        await dbContext.Set<Venue>()
            .Where(v => v.OrganizationId == organizationId)
            .ToListAsync(cancellationToken);

    public void Add(Venue venue)
    {
        dbContext.Set<Venue>().Add(venue);
        dbContext.SaveChanges();
    }
}
using Domain.Entities;
using Domain.Repositories.Venues;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Venues;

public class VenueRepository(ApplicationDbContext dbContext) : IVenueRepository
{
    public async Task<Venue?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<Venue>()
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async Task<Venue?> GetByIdWithServicesAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.Set<Venue>()
            .Include(v => v.Services)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

    public async Task<List<Venue>> GetByOrganizationId(
        Guid organizationId,
        CancellationToken cancellationToken = default) =>
        await dbContext.Set<Venue>()
            .Where(v => v.OrganizationId == organizationId)
            .ToListAsync(cancellationToken);

    public void Add(Venue venue) =>
        dbContext.Set<Venue>().Add(venue);
}
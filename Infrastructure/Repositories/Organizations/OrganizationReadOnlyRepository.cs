using Domain.Entities;
using Domain.Repositories.Organizations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Organizations;

public class OrganizationReadOnlyRepository(ApplicationDbContext dbContext) : IOrganizationReadOnlyRepository
{
    public async Task<Organization?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.Set<Organization>()
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

    public async Task<bool> ExistsAsync(Guid organizationId, CancellationToken cancellationToken = default) =>
        await dbContext.Set<Organization>()
            .AsNoTracking()
            .AnyAsync(o => o.Id == organizationId, cancellationToken);

    public async Task<List<Organization>> GetAll(
        int limit,
        int offset,
        CancellationToken cancellationToken = default) =>
        await dbContext.Set<Organization>()
            .AsNoTracking()
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);
}
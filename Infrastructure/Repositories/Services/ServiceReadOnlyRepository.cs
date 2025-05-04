using Domain.Entities;
using Domain.Repositories.Services;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Services;

public class ServiceReadOnlyRepository(ApplicationDbContext dbContext) : IServiceReadOnlyRepository
{
    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.Set<Service>()
            .AsNoTracking()
            .AnyAsync(s => s.Id == id, cancellationToken);

    public async Task<List<Service>> GetByOrganizationId(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<Service>()
            .AsNoTracking()
            .Where(s => s.OrganizationId == organizationId)
            .Include(s => s.Staffs)
            .Include(s => s.Venues)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);
    }

    public async Task<Service?> GetById(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.Set<Service>()
            .AsNoTracking()
            .Include(s => s.Staffs)
            .Include(s => s.Venues)
            .AsSplitQuery()
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
}
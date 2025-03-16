using Domain.Entities;
using Domain.Repositories.Services;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Services;

public class ServiceReadOnlyRepository(ApplicationDbContext dbContext) : IServiceReadOnlyRepository
{
    public async Task<List<Service>> GetByOrganizationIdAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<Service>()
            .AsNoTracking()
            .Where(s => s.OrganizationId == organizationId)
            .ToListAsync(cancellationToken);
    }
}
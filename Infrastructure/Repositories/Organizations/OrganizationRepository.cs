using Domain.Entities;
using Domain.Repositories.Organizations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Organizations;

public class OrganizationRepository(ApplicationDbContext dbContext) : IOrganizationRepository
{
    public async Task<Organization?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.Set<Organization>()
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

    public void Add(Organization organization)
    {
        dbContext.Set<Organization>().Add(organization);
        dbContext.SaveChanges();
    }
}
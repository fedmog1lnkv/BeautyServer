using Domain.Entities;
using Domain.Repositories.Organizations;

namespace Infrastructure.Repositories.Organizations;

public class OrganizationRepository(ApplicationDbContext dbContext) : IOrganizationRepository
{
    public async Task<Organization?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void Add(Organization organization)
    {
        dbContext.Set<Organization>().Add(organization);
        dbContext.SaveChanges();
    }
}
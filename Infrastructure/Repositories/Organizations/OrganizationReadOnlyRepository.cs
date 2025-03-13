using Domain.Repositories.Organizations;

namespace Infrastructure.Repositories.Organizations;

public class OrganizationReadOnlyRepository : IOrganizationReadOnlyRepository
{
    public async Task<bool> ExistsAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
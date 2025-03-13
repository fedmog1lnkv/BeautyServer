using Domain.Entities;
using Domain.Repositories.Base;

namespace Domain.Repositories.Organizations;

public interface IOrganizationReadOnlyRepository : IReadOnlyRepository<Organization>
{
    Task<bool> ExistsAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);
}
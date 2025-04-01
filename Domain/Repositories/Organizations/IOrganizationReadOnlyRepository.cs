using Domain.Entities;
using Domain.Repositories.Base;

namespace Domain.Repositories.Organizations;

public interface IOrganizationReadOnlyRepository : IReadOnlyRepository<Organization>
{
    Task<Organization?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);

    Task<List<Organization>> GetAll(
        int limit,
        int offset,
        CancellationToken cancellationToken = default);
}
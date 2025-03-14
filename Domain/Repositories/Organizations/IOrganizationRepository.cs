using Domain.Entities;
using Domain.Repositories.Base;

namespace Domain.Repositories.Organizations;

public interface IOrganizationRepository : IRepository<Organization>
{
    Task<Organization?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(Organization organization);
}
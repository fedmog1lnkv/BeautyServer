using Domain.Entities;
using Domain.Repositories.Base;

namespace Domain.Repositories.Services;

public interface IServiceReadOnlyRepository : IReadOnlyRepository<Service>
{
    Task<List<Service>> GetByOrganizationIdAsync(Guid organizationId, CancellationToken cancellationToken = default);
}
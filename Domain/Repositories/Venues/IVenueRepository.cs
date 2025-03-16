using Domain.Entities;
using Domain.Repositories.Base;

namespace Domain.Repositories.Venues;

public interface IVenueRepository : IRepository<Venue>
{
    Task<Venue?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Venue?> GetByIdWithServicesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Venue>> GetByOrganizationId(Guid organizationId, CancellationToken cancellationToken = default);
    void Add(Venue venue);
}
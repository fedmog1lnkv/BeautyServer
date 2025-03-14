using Domain.Entities;
using Domain.Repositories.Base;

namespace Domain.Repositories.Venues;

public interface IVenueRepository : IRepository<Venue>
{
    Task<List<Venue>> GetByOrganizationId(Guid organizationId, CancellationToken cancellationToken = default);
    void Add(Venue venue);
}
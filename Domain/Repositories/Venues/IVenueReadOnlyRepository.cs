using Domain.Entities;
using Domain.Repositories.Base;

namespace Domain.Repositories.Venues;

public interface IVenueReadOnlyRepository : IReadOnlyRepository<Venue>
{
    Task<bool> ExistsAsync(
        Guid venueId,
        CancellationToken cancellationToken = default);

    Task<List<Venue>> GetByLocation(
        double latitude,
        double longitude,
        int limit,
        int offset,
        string? search,
        CancellationToken cancellationToken = default);

    Task<List<Venue>> GetAll(
        int limit,
        int offset,
        string? search,
        CancellationToken cancellationToken = default);
    
    Task<List<Venue>> GetInBounds(
        double minLatitude,
        double minLongitude,
        double maxLatitude,
        double maxLongitude,
        CancellationToken cancellationToken = default);

    Task<Venue?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Venue?> GetByIdWithServicesAsync(Guid id, CancellationToken cancellationToken = default);
}
using Domain.Entities;
using Domain.Repositories.Base;

namespace Domain.Repositories.Venues;

public interface IVenueRepository : IRepository<Venue>
{
    Task<Venue?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Venue?> GetByIdWithServicesAndPhotosAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Venue?> GetByIdWithPhotos(Guid id, CancellationToken cancellationToken = default);
    Task<List<Venue>> GetByOrganizationId(Guid organizationId, CancellationToken cancellationToken = default);
    void Add(Venue venue);
    void Remove(Venue venue);

    Task<string?> UploadPhotoAsync(string base64Photo, string fileName);
    Task<bool> RemovePhoto(string photoUrl);
}
using Domain.Entities;
using Domain.Repositories.Venues;
using Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Venues;

public class VenueRepository(ApplicationDbContext dbContext, S3StorageUtils s3StorageUtils) : IVenueRepository
{
    public async Task<Venue?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<Venue>()
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async Task<Venue?> GetByIdWithServicesAndPhotosAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        await dbContext.Set<Venue>()
            .Include(v => v.Services)
            .Include(v => v.Photos)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

    public async Task<Venue?> GetByIdWithServices(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.Set<Venue>()
            .Include(v => v.Services)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

    public async Task<Venue?> GetByIdWithPhotos(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.Set<Venue>()
            .Include(v => v.Photos)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

    public async Task<List<Venue>> GetByOrganizationId(
        Guid organizationId,
        CancellationToken cancellationToken = default) =>
        await dbContext.Set<Venue>()
            .Where(v => v.OrganizationId == organizationId)
            .ToListAsync(cancellationToken);

    public void Add(Venue venue) =>
        dbContext.Set<Venue>().Add(venue);

    public void Remove(Venue venue) =>
        dbContext.Set<Venue>().Remove(venue);

    public async Task<string?> UploadPhotoAsync(string base64Photo, string fileName) =>
        await s3StorageUtils.UploadPhotoAsync(base64Photo, fileName, "venues");

    public async Task<bool> DeletePhoto(string photoUrl) =>
        await s3StorageUtils.DeletePhoto(photoUrl, "venues");
}
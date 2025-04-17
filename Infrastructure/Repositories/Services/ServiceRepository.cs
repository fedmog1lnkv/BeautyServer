using Domain.Entities;
using Domain.Repositories.Services;
using Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Services;

public class ServiceRepository(ApplicationDbContext dbContext, S3StorageUtils s3StorageUtils) : IServiceRepository
{
    public async Task<Service?> GetById(Guid serviceId, CancellationToken cancellationToken = default) =>
        await dbContext.Set<Service>()
            .FirstOrDefaultAsync(s => s.Id == serviceId, cancellationToken);

    public async Task<List<Service>> GetByOrganizationId(
        Guid organizationId,
        CancellationToken cancellationToken = default) =>
        await dbContext.Set<Service>()
            .Where(s => s.OrganizationId == organizationId)
            .ToListAsync(cancellationToken);

    public void Add(Service service) =>
        dbContext.Set<Service>().Add(service);

    public void Remove(Service service) =>
        dbContext.Set<Service>().Remove(service);

    public async Task<string?> UploadPhotoAsync(string base64Photo, string fileName) =>
        await s3StorageUtils.UploadPhotoAsync(base64Photo, fileName, "services");
    
    public async Task<bool> DeletePhoto(string photoUrl) =>
        await s3StorageUtils.DeletePhoto(photoUrl, "services");
}
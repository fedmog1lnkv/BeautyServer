using Domain.Entities;
using Domain.Repositories.Organizations;
using Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repositories.Organizations;

public class OrganizationRepository(
    ApplicationDbContext dbContext,
    S3StorageUtils s3StorageUtils,
    IConfiguration configuration)
    : IOrganizationRepository
{
    private readonly string _bucketName = configuration["AWS:BucketName"]!;

    public async Task<Organization?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.Set<Organization>()
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

    public void Add(Organization organization) =>
        dbContext.Set<Organization>().Add(organization);

    public void Remove(Organization organization) =>
        dbContext.Set<Organization>().Remove(organization);

    public async Task<string?> UploadPhotoAsync(string base64Photo, string fileName) =>
        await s3StorageUtils.UploadPhotoAsync(base64Photo, fileName, "organizations");
}
using Domain.Entities;
using Domain.Repositories.Staffs;
using Domain.ValueObjects;
using Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Staffs;

public class StaffRepository(ApplicationDbContext dbContext, S3StorageUtils s3StorageUtils) : IStaffRepository
{
    public async Task<Staff?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.Set<Staff>()
            .Include(s => s.Services)
            .Include(s => s.TimeSlots)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<Staff?> GetByIdWithServicesAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        await dbContext.Set<Staff>()
            .Include(s => s.Services)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<Staff?> GetByIdWithTimeSlotsAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.Set<Staff>()
            .Include(s => s.TimeSlots)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<Staff?> GetByPhoneNumberAsync(
        StaffPhoneNumber phoneNumber,
        CancellationToken cancellationToken = default) =>
        await dbContext.Set<Staff>()
            .FirstOrDefaultAsync(s => s.PhoneNumber == phoneNumber, cancellationToken);

    public void Add(Staff staff) =>
        dbContext.Set<Staff>().Add(staff);

    public async Task<string?> UploadPhotoAsync(string base64Photo, string fileName) =>
        await s3StorageUtils.UploadPhotosAsync(base64Photo, fileName, "staffs");

    public async Task<bool> DeletePhoto(string photoUrl) =>
        await s3StorageUtils.DeletePhoto(photoUrl, "staffs");

    public void Remove(Staff staff) =>
        dbContext.Set<Staff>().Remove(staff);

    public async Task<List<Staff>> GetByOrganizationId(
        Guid organizationId,
        CancellationToken cancellationToken =
            default) =>
        await dbContext.Set<Staff>()
            .Where(s => s.OrganizationId == organizationId)
            .ToListAsync(cancellationToken);
}
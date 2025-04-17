using Domain.Entities;
using Domain.Repositories.Users;
using Domain.ValueObjects;
using Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Users;

internal sealed class UserRepository(ApplicationDbContext dbContext, S3StorageUtils s3StorageUtils) : IUserRepository
{
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<User>()
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public async Task<User?> GetByPhoneNumberAsync(
        UserPhoneNumber phoneNumber,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<User>()
            .FirstOrDefaultAsync(user => user.PhoneNumber == phoneNumber, cancellationToken);
    }

    public void Add(User user) =>
        dbContext.Set<User>().Add(user);

    public void Remove(User user) =>
        dbContext.Set<User>().Remove(user);

    public async Task<string?> UploadPhotoAsync(string base64Photo, string fileName) =>
        await s3StorageUtils.UploadPhotoAsync(base64Photo, fileName, "users");
    
    public async Task<bool> DeletePhoto(string photoUrl) =>
        await s3StorageUtils.DatelePhoto(photoUrl, "users");
}
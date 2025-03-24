using Domain.Entities;
using Domain.Repositories.Users;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Users;

internal sealed class UserReadOnlyRepository(ApplicationDbContext dbContext) : IUserReadOnlyRepository
{
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.Set<User>()
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.Set<User>()
            .AsNoTracking()
            .AnyAsync(u => u.Id == id, cancellationToken);

    public async Task<bool> IsPhoneNumberUnique(
        UserPhoneNumber phoneNumber,
        CancellationToken cancellationToken = default)
    {
        var userExists = await dbContext.Set<User>()
            .AsNoTracking()
            .AnyAsync(user => user.PhoneNumber == phoneNumber, cancellationToken);

        return !userExists;
    }
}
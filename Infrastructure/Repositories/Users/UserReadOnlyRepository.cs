using Domain.Repositories.Users;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Users;

internal sealed class UserReadOnlyRepository(ApplicationDbContext dbContext) : IUserReadOnlyRepository
{
    public async Task<bool> IsPhoneNumberUnique(
        UserPhoneNumber phoneNumber,
        CancellationToken cancellationToken = default)
    {
        var userExists = await dbContext.Set<Domain.Entities.User>()
            .AsNoTracking()
            .AnyAsync(user => user.PhoneNumber == phoneNumber, cancellationToken);

        return !userExists;
    }
}
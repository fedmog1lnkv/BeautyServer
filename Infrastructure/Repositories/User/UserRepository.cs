using Domain.Repositories.Users;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.User;

internal sealed class UserRepository(ApplicationDbContext dbContext) : IUserRepository
{
    public async Task<Domain.Entities.User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<Domain.Entities.User>()
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public async Task<Domain.Entities.User?> GetByPhoneNumberAsync(
        UserPhoneNumber phoneNumber,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<Domain.Entities.User>()
            .FirstOrDefaultAsync(user => user.PhoneNumber == phoneNumber, cancellationToken);
    }

    public void Add(Domain.Entities.User user)
    {
        dbContext.Set<Domain.Entities.User>().Add(user);
        dbContext.SaveChanges();
    }

    public void Remove(Domain.Entities.User user)
    {
        dbContext.Set<Domain.Entities.User>().Remove(user);
        dbContext.SaveChanges();
    }
}
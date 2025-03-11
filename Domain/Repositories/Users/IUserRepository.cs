using Domain.Entities;
using Domain.Repositories.Base;
using Domain.ValueObjects;

namespace Domain.Repositories.Users;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(User user);
    void Remove(User user);
    Task<User?> GetByPhoneNumberAsync(UserPhoneNumber phoneNumber, CancellationToken cancellationToken = default);
}
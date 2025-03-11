using Domain.Entities;
using Domain.Repositories.Base;
using Domain.ValueObjects;

namespace Domain.Repositories.Users;

public interface IUserReadOnlyRepository : IReadOnlyRepository<User>
{
    Task<bool> IsPhoneNumberUnique(UserPhoneNumber phoneNumber, CancellationToken cancellationToken = default);
}
using Domain.Errors;
using Domain.Primitives;
using Domain.Repositories.Users;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entities;

public sealed class User : AggregateRoot, IAuditableEntity
{
    private User(
        Guid id,
        UserName name,
        UserPhoneNumber phoneNumber,
        DateTime createdOnUtc) : base(id)
    {
        Name = name;
        PhoneNumber = phoneNumber;
        
        CreatedOnUtc = createdOnUtc;
    }

    public UserName Name { get; private set; }
    public UserPhoneNumber PhoneNumber { get; private set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    public static async Task<Result<User>> CreateAsync(
        Guid id,
        string name,
        string phoneNumber,
        DateTime createdOnUtc,
        IUserReadOnlyRepository repository,
        CancellationToken cancellationToken)
    {
        var createNameResult = UserName.Create(name);
        if (createNameResult.IsFailure)
            return Result.Failure<User>(createNameResult.Error);

        var createPhoneNumberResult = UserPhoneNumber.Create(phoneNumber);
        if (createPhoneNumberResult.IsFailure)
            return Result.Failure<User>(createPhoneNumberResult.Error);

        var isUniquePhone = await repository.IsPhoneNumberUnique(createPhoneNumberResult.Value, cancellationToken);
        if (!isUniquePhone)
            return Result.Failure<User>(DomainErrors.User.PhoneNumberNotUnique);

        var user = new User(
            id,
            createNameResult.Value,
            createPhoneNumberResult.Value,
            createdOnUtc);

        return Result.Success(user);
    }
}
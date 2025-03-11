using Domain.Errors;
using Domain.Primitives;
using Domain.Shared;

namespace Domain.ValueObjects;

public sealed class UserName : ValueObject
{
    public const int MaxLength = 20;
    public const int MinLength = 3;

    private UserName(string value) =>
        Value = value;

    public string Value { get; }

    public static Result<UserName> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<UserName>(DomainErrors.UserName.Empty);

        if (name.Length < MinLength)
            return Result.Failure<UserName>(DomainErrors.UserName.TooShort);

        if (name.Length > MaxLength)
            return Result.Failure<UserName>(DomainErrors.UserName.TooLong);

        return new UserName(name);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
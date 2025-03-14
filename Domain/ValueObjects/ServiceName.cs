using Domain.Errors;
using Domain.Primitives;
using Domain.Shared;

namespace Domain.ValueObjects;

public sealed class ServiceName : ValueObject
{
    public const int MaxLength = 50;
    public const int MinLength = 3;

    private ServiceName(string value) =>
        Value = value;

    public string Value { get; }

    public static Result<ServiceName> Create(string name)
    {
        name = name.Trim();
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<ServiceName>(DomainErrors.ServiceName.Empty);

        if (name.Length < MinLength)
            return Result.Failure<ServiceName>(DomainErrors.ServiceName.TooShort);

        if (name.Length > MaxLength)
            return Result.Failure<ServiceName>(DomainErrors.ServiceName.TooLong);

        return new ServiceName(name);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
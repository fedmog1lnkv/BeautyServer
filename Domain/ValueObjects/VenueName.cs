using Domain.Errors;
using Domain.Primitives;
using Domain.Shared;

namespace Domain.ValueObjects;

public sealed class VenueName : ValueObject
{
    public const int MaxLength = 50;
    public const int MinLength = 3;

    private VenueName(string value) =>
        Value = value;

    public string Value { get; }

    public static Result<VenueName> Create(string name)
    {
        name = name.Trim();
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<VenueName>(DomainErrors.VenueName.Empty);

        if (name.Length < MinLength)
            return Result.Failure<VenueName>(DomainErrors.VenueName.TooShort);

        if (name.Length > MaxLength)
            return Result.Failure<VenueName>(DomainErrors.VenueName.TooLong);

        return new VenueName(name);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
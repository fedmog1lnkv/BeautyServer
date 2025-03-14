using Domain.Errors;
using Domain.Primitives;
using Domain.Shared;

namespace Domain.ValueObjects;

public sealed class StaffName : ValueObject
{
    public const int MaxLength = 20;
    public const int MinLength = 3;

    private StaffName(string value) =>
        Value = value;

    public string Value { get; }

    public static Result<StaffName> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<StaffName>(DomainErrors.StaffName.Empty);

        if (name.Length < MinLength)
            return Result.Failure<StaffName>(DomainErrors.StaffName.TooShort);

        if (name.Length > MaxLength)
            return Result.Failure<StaffName>(DomainErrors.StaffName.TooLong);

        return new StaffName(name);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
using Domain.Errors;
using Domain.Primitives;
using Domain.Shared;

namespace Domain.ValueObjects;

public sealed class CouponName : ValueObject
{
    public const int MaxLength = 50;
    public const int MinLength = 3;

    private CouponName(string value) =>
        Value = value;

    public string Value { get; }

    public static Result<CouponName> Create(string name)
    {
        name = name.Trim();
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<CouponName>(DomainErrors.CouponName.Empty);

        if (name.Length < MinLength)
            return Result.Failure<CouponName>(DomainErrors.CouponName.TooShort);

        if (name.Length > MaxLength)
            return Result.Failure<CouponName>(DomainErrors.CouponName.TooLong);

        return new CouponName(name);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
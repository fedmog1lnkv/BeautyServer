using Domain.Errors;
using Domain.Primitives;
using Domain.Shared;

namespace Domain.ValueObjects;

public sealed class CouponCode : ValueObject
{
    public const int Length = 10;

    private CouponCode(string value) =>
        Value = value;

    public string Value { get; }

    public static Result<CouponCode> Create(string name)
    {
        name = name.Trim().ToUpper();
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<CouponCode>(DomainErrors.CouponCode.Empty);

        if (name.Length != Length)
            return Result.Failure<CouponCode>(DomainErrors.CouponCode.InvalidLength);

        return new CouponCode(name);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
using Domain.Errors;
using Domain.Primitives;
using Domain.Shared;

namespace Domain.ValueObjects;

public sealed class CouponDiscountValue : ValueObject
{
    private CouponDiscountValue(decimal value) =>
        Value = value;

    public decimal Value { get; }

    public static Result<CouponDiscountValue> Create(decimal value)
    {
        if (value <= 0)
            return Result.Failure<CouponDiscountValue>(DomainErrors.CouponDiscountValue.MustBeGreaterThanZero);

        return new CouponDiscountValue(value);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
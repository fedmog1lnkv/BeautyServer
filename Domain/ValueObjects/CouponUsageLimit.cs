using Domain.Errors;
using Domain.Primitives;
using Domain.Shared;

namespace Domain.ValueObjects;

public sealed class CouponUsageLimit : ValueObject
{
    private CouponUsageLimit(int total, int remaining)
    {
        Total = total;
        Remaining = remaining;
    }

    public int Total { get; }
    public int Remaining { get; }

    public static Result<CouponUsageLimit> Create(int total, int remaining)
    {
        if (total < 0 || remaining < 0 || remaining > total)
            return Result.Failure<CouponUsageLimit>(DomainErrors.CouponUsageLimit.InvalidUsageLimit);

        return Result.Success(new CouponUsageLimit(total, remaining));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Total;
        yield return Remaining;
    }
}
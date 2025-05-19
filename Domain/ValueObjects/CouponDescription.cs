using Domain.Errors;
using Domain.Primitives;
using Domain.Shared;

namespace Domain.ValueObjects;

public class CouponDescription : ValueObject
{
    public const int MaxLength = 150;
    public const int MinLength = 3;

    private CouponDescription(string value) =>
        Value = value;

    public string Value { get; }

    public static Result<CouponDescription> Create(string description)
    {
        description = description.Trim();
        if (string.IsNullOrWhiteSpace(description))
            return Result.Failure<CouponDescription>(DomainErrors.CouponDescription.Empty);

        if (description.Length < MinLength)
            return Result.Failure<CouponDescription>(DomainErrors.CouponDescription.TooShort);

        if (description.Length > MaxLength)
            return Result.Failure<CouponDescription>(DomainErrors.CouponDescription.TooLong);

        return new CouponDescription(description);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
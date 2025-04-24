using Domain.Errors;
using Domain.Primitives;
using Domain.Shared;

namespace Domain.ValueObjects;

public sealed class StaffRating : ValueObject
{
    public const int MinRating = 0;
    public const int MaxRating = 10;
    private StaffRating(double value) =>
        Value = value;

    public double Value { get; }

    public static Result<StaffRating> Create(double rating)
    {
        if (rating is < MinRating or > MaxRating)
            return Result.Failure<StaffRating>(DomainErrors.StaffRating.InvalidRating);

        return new StaffRating(rating);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
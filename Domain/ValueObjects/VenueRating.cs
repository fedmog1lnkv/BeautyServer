using Domain.Errors;
using Domain.Primitives;
using Domain.Shared;

namespace Domain.ValueObjects;

public sealed class VenueRating : ValueObject
{
    public const int MinRating = 0;
    public const int MaxRating = 10;
    private VenueRating(double value) =>
        Value = value;

    public double Value { get; }

    public static Result<VenueRating> Create(double rating)
    {
        if (rating is < MinRating or > MaxRating)
            return Result.Failure<VenueRating>(DomainErrors.VenueRating.InvalidRating);

        return new VenueRating(rating);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
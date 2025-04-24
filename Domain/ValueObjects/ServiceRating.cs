using Domain.Errors;
using Domain.Primitives;
using Domain.Shared;

namespace Domain.ValueObjects;

public sealed class ServiceRating : ValueObject
{
    public const int MinRating = 0;
    public const int MaxRating = 10;
    private ServiceRating(double value) =>
        Value = value;

    public double Value { get; }

    public static Result<ServiceRating> Create(double rating)
    {
        if (rating is < MinRating or > MaxRating)
            return Result.Failure<ServiceRating>(DomainErrors.ServiceRating.InvalidRating);

        return new ServiceRating(rating);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
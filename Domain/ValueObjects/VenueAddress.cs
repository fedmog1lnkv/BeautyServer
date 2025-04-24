using Domain.Errors;
using Domain.Primitives;
using Domain.Shared;

namespace Domain.ValueObjects;

public class VenueAddress : ValueObject
{
    public const int MaxLength = 150;
    public const int MinLength = 3;

    private VenueAddress(string value) =>
        Value = value;

    public string Value { get; }

    public static Result<VenueAddress> Create(string description)
    {
        description = description.Trim();
        if (string.IsNullOrWhiteSpace(description))
            return Result.Failure<VenueAddress>(DomainErrors.VenueAddress.Empty);

        if (description.Length < MinLength)
            return Result.Failure<VenueAddress>(DomainErrors.VenueAddress.TooShort);

        if (description.Length > MaxLength)
            return Result.Failure<VenueAddress>(DomainErrors.VenueAddress.TooLong);

        return new VenueAddress(description);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
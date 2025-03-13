using Domain.Errors;
using Domain.Primitives;
using Domain.Shared;

namespace Domain.ValueObjects;

public class VenueDescription : ValueObject
{
    public const int MaxLength = 50;
    public const int MinLength = 3;

    private VenueDescription(string value) =>
        Value = value;

    public string Value { get; }

    public static Result<VenueDescription> Create(string description)
    {
        description = description.Trim();
        if (string.IsNullOrWhiteSpace(description))
            return Result.Failure<VenueDescription>(DomainErrors.VenueDescription.Empty);

        if (description.Length < MinLength)
            return Result.Failure<VenueDescription>(DomainErrors.VenueDescription.TooShort);

        if (description.Length > MaxLength)
            return Result.Failure<VenueDescription>(DomainErrors.VenueDescription.TooLong);

        return new VenueDescription(description);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
using Domain.Errors;
using Domain.Primitives;
using Domain.Shared;

namespace Domain.ValueObjects;

public class OrganizationDescription : ValueObject
{
    public const int MaxLength = 50;
    public const int MinLength = 3;

    private OrganizationDescription(string value) =>
        Value = value;

    public string Value { get; }

    public static Result<OrganizationDescription> Create(string description)
    {
        description = description.Trim();
        if (string.IsNullOrWhiteSpace(description))
            return Result.Failure<OrganizationDescription>(DomainErrors.OrganizationDescription.Empty);

        if (description.Length < MinLength)
            return Result.Failure<OrganizationDescription>(DomainErrors.OrganizationDescription.TooShort);

        if (description.Length > MaxLength)
            return Result.Failure<OrganizationDescription>(DomainErrors.OrganizationDescription.TooLong);

        return new OrganizationDescription(description);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
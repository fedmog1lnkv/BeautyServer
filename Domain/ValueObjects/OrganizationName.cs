using Domain.Errors;
using Domain.Primitives;
using Domain.Shared;

namespace Domain.ValueObjects;

public sealed class OrganizationName : ValueObject
{
    public const int MaxLength = 50;
    public const int MinLength = 3;

    private OrganizationName(string value) =>
        Value = value;

    public string Value { get; }

    public static Result<OrganizationName> Create(string name)
    {
        name = name.Trim();
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<OrganizationName>(DomainErrors.OrganizationName.Empty);

        if (name.Length < MinLength)
            return Result.Failure<OrganizationName>(DomainErrors.OrganizationName.TooShort);

        if (name.Length > MaxLength)
            return Result.Failure<OrganizationName>(DomainErrors.OrganizationName.TooLong);

        return new OrganizationName(name);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
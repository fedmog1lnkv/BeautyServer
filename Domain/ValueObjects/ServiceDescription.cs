using Domain.Errors;
using Domain.Primitives;
using Domain.Shared;

namespace Domain.ValueObjects;

public class ServiceDescription : ValueObject
{
    public const int MaxLength = 1000;
    public const int MinLength = 3;

    private ServiceDescription(string value) =>
        Value = value;

    public string Value { get; }

    public static Result<ServiceDescription> Create(string description)
    {
        description = description.Trim();
        if (string.IsNullOrWhiteSpace(description))
            return Result.Failure<ServiceDescription>(DomainErrors.ServiceDescription.Empty);

        if (description.Length < MinLength)
            return Result.Failure<ServiceDescription>(DomainErrors.ServiceDescription.TooShort);

        if (description.Length > MaxLength)
            return Result.Failure<ServiceDescription>(DomainErrors.ServiceDescription.TooLong);

        return new ServiceDescription(description);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
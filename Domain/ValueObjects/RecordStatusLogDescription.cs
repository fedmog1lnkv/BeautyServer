using Domain.Errors;
using Domain.Primitives;
using Domain.Shared;

namespace Domain.ValueObjects;

public class RecordStatusLogDescription : ValueObject
{
    public const int MaxLength = 150;
    public const int MinLength = 3;

    private RecordStatusLogDescription(string value) =>
        Value = value;

    public string Value { get; }

    public static Result<RecordStatusLogDescription> Create(string description)
    {
        description = description.Trim();
        if (string.IsNullOrWhiteSpace(description))
            return Result.Failure<RecordStatusLogDescription>(DomainErrors.RecordStatusLogDescription.Empty);

        if (description.Length < MinLength)
            return Result.Failure<RecordStatusLogDescription>(DomainErrors.RecordStatusLogDescription.TooShort);

        if (description.Length > MaxLength)
            return Result.Failure<RecordStatusLogDescription>(DomainErrors.RecordStatusLogDescription.TooLong);

        return new RecordStatusLogDescription(description);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
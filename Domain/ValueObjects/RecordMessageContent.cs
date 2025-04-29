using Domain.Errors;
using Domain.Primitives;
using Domain.Shared;

namespace Domain.ValueObjects;

public class RecordMessageContent : ValueObject
{
    public const int MaxLength = 500;
    public const int MinLength = 1;

    private RecordMessageContent(string value) =>
        Value = value;

    public string Value { get; }

    public static Result<RecordMessageContent> Create(string message)
    {
        message = message.Trim();
        if (string.IsNullOrWhiteSpace(message))
            return Result.Failure<RecordMessageContent>(DomainErrors.RecordMessage.Empty);

        if (message.Length < MinLength)
            return Result.Failure<RecordMessageContent>(DomainErrors.RecordMessage.TooShort);

        if (message.Length > MaxLength)
            return Result.Failure<RecordMessageContent>(DomainErrors.RecordMessage.TooLong);

        return new RecordMessageContent(message);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
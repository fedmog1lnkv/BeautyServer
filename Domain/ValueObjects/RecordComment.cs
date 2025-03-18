using Domain.Errors;
using Domain.Primitives;
using Domain.Shared;

namespace Domain.ValueObjects;

public sealed class RecordComment : ValueObject
{
    public const int MaxLength = 150;
    public const int MinLength = 10;

    private RecordComment(string value) =>
        Value = value;

    public string Value { get; }

    public static Result<RecordComment> Create(string text)
    {
        text = text.Trim();
        if (string.IsNullOrWhiteSpace(text))
            return Result.Failure<RecordComment>(DomainErrors.RecordComment.Empty);

        if (text.Length < MinLength)
            return Result.Failure<RecordComment>(DomainErrors.RecordComment.TooShort);

        if (text.Length > MaxLength)
            return Result.Failure<RecordComment>(DomainErrors.RecordComment.TooLong);

        return new RecordComment(text);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
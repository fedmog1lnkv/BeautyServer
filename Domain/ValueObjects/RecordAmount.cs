using Domain.Errors;
using Domain.Primitives;
using Domain.Shared;

namespace Domain.ValueObjects;

public sealed class RecordAmount : ValueObject
{
    private RecordAmount(decimal value) => Value = value;

    public decimal Value { get; }

    public static Result<RecordAmount> Create(decimal value)
    {
        if (value < 0)
            return Result.Failure<RecordAmount>(DomainErrors.RecordAmount.InvalidOriginalPrice);

        return Result.Success(new RecordAmount(value));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
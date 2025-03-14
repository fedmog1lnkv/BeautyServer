using Domain.Errors;
using Domain.Primitives;
using Domain.Shared;

namespace Domain.ValueObjects;

public sealed class ServicePrice : ValueObject
{
    private ServicePrice(double value) =>
        Value = value;

    public double Value { get; }

    public static Result<ServicePrice> Create(double price)
    {
        return price <= 0 
            ? Result.Failure<ServicePrice>(DomainErrors.ServicePrice.TooLow)
            : new ServicePrice(price);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
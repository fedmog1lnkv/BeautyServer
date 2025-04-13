using Domain.Errors;
using Domain.Primitives;
using Domain.Shared;

namespace Domain.ValueObjects;

public class ServicePhoto : ValueObject
{
    public const int MaxLength = 2048;

    public string Value { get; }

    private ServicePhoto(string value) =>
        Value = value;

    public static Result<ServicePhoto> Create(string photo)
    {
        photo = photo.Trim();

        if (string.IsNullOrWhiteSpace(photo))
            return Result.Failure<ServicePhoto>(DomainErrors.ServicePhoto.Empty);

        if (photo.Length > MaxLength)
            return Result.Failure<ServicePhoto>(DomainErrors.ServicePhoto.TooLong);

        if (!IsValidUrl(photo))
            return Result.Failure<ServicePhoto>(DomainErrors.ServicePhoto.InvalidFormat);

        return new ServicePhoto(photo);
    }

    private static bool IsValidUrl(string input)
    {
        return Uri.TryCreate(input, UriKind.Absolute, out _);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
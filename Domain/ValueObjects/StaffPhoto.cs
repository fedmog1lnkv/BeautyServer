using Domain.Errors;
using Domain.Primitives;
using Domain.Shared;

namespace Domain.ValueObjects;

public class StaffPhoto : ValueObject
{
    public const int MaxLength = 2048;

    public string Value { get; }

    private StaffPhoto(string value) =>
        Value = value;

    public static Result<StaffPhoto> Create(string photo)
    {
        photo = photo.Trim();

        if (string.IsNullOrWhiteSpace(photo))
            return Result.Failure<StaffPhoto>(DomainErrors.StaffPhoto.Empty);

        if (photo.Length > MaxLength)
            return Result.Failure<StaffPhoto>(DomainErrors.StaffPhoto.TooLong);

        if (!IsValidUrl(photo))
            return Result.Failure<StaffPhoto>(DomainErrors.StaffPhoto.InvalidFormat);

        return new StaffPhoto(photo);
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
using Domain.Errors;
using Domain.Primitives;
using Domain.Shared;

namespace Domain.ValueObjects;

public class UserPhoto : ValueObject
{
    public const int MaxLength = 2048;

    public string Value { get; }

    private UserPhoto(string value) =>
        Value = value;

    public static Result<UserPhoto> Create(string photo)
    {
        photo = photo.Trim();

        if (string.IsNullOrWhiteSpace(photo))
            return Result.Failure<UserPhoto>(DomainErrors.UserPhoto.Empty);

        if (photo.Length > MaxLength)
            return Result.Failure<UserPhoto>(DomainErrors.UserPhoto.TooLong);

        if (!IsValidUrl(photo))
            return Result.Failure<UserPhoto>(DomainErrors.UserPhoto.InvalidFormat);

        return new UserPhoto(photo);
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
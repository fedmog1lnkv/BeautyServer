using Domain.Errors;
using Domain.Primitives;
using Domain.Shared;

namespace Domain.ValueObjects;

public sealed class Location : ValueObject
{
    public static Location Empty => new(0, 0);

    public const double MinLatitude = -90;
    public const double MaxLatitude = 90;
    public const double MinLongitude = -180;
    public const double MaxLongitude = 180;

    private Location(
        double latitude,
        double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public double Latitude { get; private set; }
    public double Longitude { get; private set; }

    public static Result<Location> Create(
        double latitude,
        double longitude)
    {
        if (double.IsNaN(latitude) || double.IsInfinity(latitude))
            return Result.Failure<Location>(DomainErrors.Location.InvalidLatitude);

        if (double.IsNaN(longitude) || double.IsInfinity(longitude))
            return Result.Failure<Location>(DomainErrors.Location.InvalidLongitude);

        if (latitude < MinLatitude || latitude > MaxLatitude)
            return Result.Failure<Location>(DomainErrors.Location.LatitudeOutOfRange);

        if (longitude < MinLongitude || longitude > MaxLongitude)
            return Result.Failure<Location>(DomainErrors.Location.LongitudeOutOfRange);

        return new Location(latitude, longitude);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Latitude;
        yield return Longitude;
    }
}
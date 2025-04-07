using Domain.Repositories.Utils;
using GeoTimeZone;

namespace Infrastructure.Repositories.Utils;

public class LocationRepository : ILocationRepository
{
    public TimeZoneInfo GetTimeZoneByLocation(double latitude, double longitude)
    {
        var timeZoneId = TimeZoneLookup.GetTimeZone(latitude, longitude).Result;
        return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
    }
}
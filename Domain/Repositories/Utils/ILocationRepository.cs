namespace Domain.Repositories.Utils;

public interface ILocationRepository
{
    TimeZoneInfo GetTimeZoneByLocation(double latitude, double longitude);
}
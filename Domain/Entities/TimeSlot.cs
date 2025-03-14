using Domain.Errors;
using Domain.Primitives;
using Domain.Repositories.Venues;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entities;

public class TimeSlot : Entity
{
    private TimeSlot(
        Guid id,
        Guid staffId,
        Guid venueId,
        DayOfWeek weekday,
        List<Interval> intervals) : base(id)
    {
        StaffId = staffId;
        VenueId = venueId;
        Weekday = weekday;
        Intervals = intervals;
    }
    
#pragma warning disable CS8618
    private TimeSlot() { }
#pragma warning restore CS8618

    public Guid StaffId { get; private set; }
    public Guid VenueId { get; private set; }
    public DayOfWeek Weekday { get; private set; }
    public List<Interval> Intervals { get; private set; }
    public Venue Venue { get; private set; }

    public static async Task<Result<TimeSlot>> Create(
        Guid id,
        Guid staffId,
        Guid venueId,
        DayOfWeek weekday,
        IVenueReadOnlyRepository venueRepository)
    {
        var venueExists = await venueRepository.ExistsAsync(venueId);
        if (!venueExists)
            return Result.Failure<TimeSlot>(DomainErrors.Venue.NotFound(venueId));

        return new TimeSlot(id, staffId, venueId, weekday, new List<Interval>());
    }

    public Result UpdateIntervals(List<Interval> intervals)
    {
        for (int i = 0; i < intervals.Count; i++)
        {
            for (int j = i + 1; j < intervals.Count; j++)
            {
                if (intervals[i].Overlaps(intervals[j]))
                {
                    return Result.Failure(DomainErrors.TimeSlot.IntervalsOverlap);
                }
            }
        }

        Intervals = intervals;
        return Result.Success();
    }
}
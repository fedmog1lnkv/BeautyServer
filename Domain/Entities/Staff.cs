using Domain.Enums;
using Domain.Errors;
using Domain.Primitives;
using Domain.Repositories.Organizations;
using Domain.Repositories.Staffs;
using Domain.Repositories.Venues;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entities;

public class Staff : AggregateRoot, IAuditableEntity
{
    private readonly List<Service> _services = [];
    private readonly List<TimeSlot> _timeSlots = [];

    private Staff(
        Guid id,
        Guid organizationId,
        StaffName name,
        StaffPhoneNumber phoneNumber,
        StaffRole role,
        StaffRating rating,
        StaffPhoto? photo,
        StaffSettings settings,
        DateTime createdOnUtc) : base(id)
    {
        OrganizationId = organizationId;
        Name = name;
        PhoneNumber = phoneNumber;
        Role = role;
        Rating = rating;
        Photo = photo;
        Settings = settings;

        CreatedOnUtc = createdOnUtc;
    }

#pragma warning disable CS8618
    private Staff() { }
#pragma warning restore CS8618

    public Guid OrganizationId { get; private set; }
    public StaffName Name { get; private set; }
    public StaffPhoneNumber PhoneNumber { get; private set; }
    public StaffRole Role { get; private set; }
    public StaffPhoto? Photo { get; private set; }
    public StaffSettings Settings { get; private set; }
    public StaffRating Rating { get; private set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }
    public IReadOnlyCollection<Service> Services => _services.AsReadOnly();
    public IReadOnlyCollection<TimeSlot> TimeSlots => _timeSlots.AsReadOnly();

    public static async Task<Result<Staff>> CreateAsync(
        Guid id,
        Guid organizationId,
        string name,
        string phoneNumber,
        DateTime createdOnUtc,
        IStaffReadOnlyRepository staffRepository,
        IOrganizationReadOnlyRepository organizationRepository,
        CancellationToken cancellationToken)
    {
        if (organizationId == Guid.Empty)
            return Result.Failure<Staff>(DomainErrors.Staff.OrganizationIdEmpty);

        var organizationExists = await organizationRepository.ExistsAsync(organizationId, cancellationToken);
        if (!organizationExists)
            return Result.Failure<Staff>(DomainErrors.Organization.NotFound(organizationId));

        var staffPhoneResult = StaffPhoneNumber.Create(phoneNumber);
        if (staffPhoneResult.IsFailure)
            return Result.Failure<Staff>(staffPhoneResult.Error);

        var isUniquePhone = await staffRepository.IsPhoneNumberUnique(staffPhoneResult.Value, cancellationToken);
        if (!isUniquePhone)
            return Result.Failure<Staff>(DomainErrors.Staff.PhoneNumberNotUnique);

        var staffNameResult = StaffName.Create(name);
        if (staffNameResult.IsFailure)
            return Result.Failure<Staff>(staffNameResult.Error);

        var createSettingsResult = StaffSettings.Create(null);
        if (createSettingsResult.IsFailure)
            return Result.Failure<Staff>(createSettingsResult.Error);

        var createRatingResult = StaffRating.Create(0);
        if (createRatingResult.IsFailure)
            return Result.Failure<Staff>(createRatingResult.Error);

        return new Staff(
            id,
            organizationId,
            staffNameResult.Value,
            staffPhoneResult.Value,
            StaffRole.Unknown,
            createRatingResult.Value,
            null,
            createSettingsResult.Value,
            createdOnUtc);
    }

    public Result SetName(string name)
    {
        var nameResult = StaffName.Create(name);
        if (nameResult.IsFailure)
            return nameResult;

        if (Name.Equals(nameResult.Value))
            return Result.Success();

        Name = nameResult.Value;
        return Result.Success();
    }

    public Result SetFirebaseToken(string? firebaseToken)
    {
        var settingsResult = StaffSettings.Create(firebaseToken);

        if (settingsResult.IsFailure)
            return settingsResult;

        Settings = settingsResult.Value;

        return Result.Success();
    }

    public Result SetRole(StaffRole role)
    {
        if (Role == role)
            return Result.Success();

        Role = role;
        return Result.Success();
    }

    public Result SetRating(double rating)
    {
        var ratingResult = StaffRating.Create(rating);
        if (ratingResult.IsFailure)
            return ratingResult;

        if (Rating.Equals(ratingResult.Value))
            return Result.Success();

        Rating = ratingResult.Value;
        return Result.Success();
    }

    public Result SetPhoto(string photoUrl)
    {
        var photoResult = StaffPhoto.Create(photoUrl);
        if (photoResult.IsFailure)
            return photoResult;

        if (Photo is not null && Photo.Equals(photoResult.Value))
            return Result.Success();

        Photo = photoResult.Value;
        return Result.Success();
    }

    public async Task<Result> AddTimeSlotAsync(
        Guid id,
        Guid venueId,
        DateOnly date,
        IVenueReadOnlyRepository venueReadOnlyRepository,
        CancellationToken cancellationToken)
    {
        if (TimeSlots.Any(ts => ts.Id == id || ts.Date == date && ts.VenueId == venueId))
            return Result.Failure(DomainErrors.TimeSlot.Overlap);

        var createTimeSlotResult = await TimeSlot.CreateAsync(
            id,
            Id,
            venueId,
            date,
            venueReadOnlyRepository,
            cancellationToken);

        if (createTimeSlotResult.IsFailure)
            return createTimeSlotResult;

        var timeSlot = createTimeSlotResult.Value;

        _timeSlots.Add(timeSlot);

        return Result.Success();
    }

    public Result DeleteTimeSlot(Guid timeSlotToRemove)
    {
        var timeSlot = _timeSlots.FirstOrDefault(ts => ts.Id == timeSlotToRemove);

        if (timeSlot == null)
            return Result.Success();

        _timeSlots.Remove(timeSlot);

        return Result.Success();
    }

    public Result AddTimeSlotInterval(
        Guid timeSlotId,
        TimeSpan start,
        TimeSpan end)
    {
        var timeSlot = _timeSlots.FirstOrDefault(ts => ts.Id == timeSlotId);
        if (timeSlot == null)
            return Result.Failure(DomainErrors.TimeSlot.NotFound(timeSlotId));

        var newIntervalResult = Interval.Create(start, end);
        if (newIntervalResult.IsFailure)
            return newIntervalResult;

        var checkOverlapResult = CheckTimeSlotIntervalsVenueOverlap(
            timeSlot.VenueId,
            timeSlot.Date,
            [newIntervalResult.Value]);
        if (checkOverlapResult.IsFailure)
            return checkOverlapResult;

        var addIntervalResult = timeSlot.AddInterval(start, end);
        return addIntervalResult.IsFailure
            ? addIntervalResult
            : Result.Success();
    }

    public Result UpdateTimeSlotIntervals(Guid timeSlotId, List<Interval> intervals)
    {
        var timeSlot = _timeSlots.FirstOrDefault(ts => ts.Id == timeSlotId);
        if (timeSlot == null)
            return Result.Failure(DomainErrors.TimeSlot.NotFound(timeSlotId));

        var checkOverlapResult = CheckTimeSlotIntervalsVenueOverlap(timeSlot.VenueId, timeSlot.Date, intervals);
        if (checkOverlapResult.IsFailure)
            return checkOverlapResult;

        var updateIntervalsResult = timeSlot.UpdateIntervals(intervals);
        return updateIntervalsResult.IsFailure
            ? updateIntervalsResult
            : Result.Success();
    }

    private Result CheckTimeSlotIntervalsVenueOverlap(
        Guid currentVenueId,
        DateOnly day,
        List<Interval> newIntervals)
    {
        var sameDayTimeSlots = _timeSlots.Where(ts => ts.Date == day && ts.VenueId != currentVenueId).ToList();

        foreach (var newInterval in newIntervals)
        {
            foreach (var timeSlot in sameDayTimeSlots)
            {
                foreach (var interval in timeSlot.Intervals)
                {
                    if (interval.Overlaps(newInterval))
                        return Result.Failure(DomainErrors.TimeSlot.OverlapVenue);
                }
            }
        }
        return Result.Success();
    }

    public Result SetServices(IEnumerable<Service> services)
    {
        var validServices = services.Where(s => s.OrganizationId == OrganizationId)
            .DistinctBy(s => s.Id)
            .ToList();

        _services.Clear();
        _services.AddRange(validServices);

        return Result.Success();
    }
}
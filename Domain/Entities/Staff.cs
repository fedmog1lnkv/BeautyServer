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
        DateTime createdOnUtc) : base(id)
    {
        OrganizationId = organizationId;
        Name = name;
        PhoneNumber = phoneNumber;
        Role = role;

        CreatedOnUtc = createdOnUtc;
    }

#pragma warning disable CS8618
    private Staff() { }
#pragma warning restore CS8618

    public Guid OrganizationId { get; private set; }
    public StaffName Name { get; private set; }
    public StaffPhoneNumber PhoneNumber { get; private set; }
    public StaffRole Role { get; private set; }
    public StaffPhoto Photo { get; private set; }
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

        return new Staff(
            id,
            organizationId,
            staffNameResult.Value,
            staffPhoneResult.Value,
            StaffRole.Unknown,
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

    public Result SetRole(StaffRole role)
    {
        if (Role == role)
            return Result.Success();

        Role = role;
        return Result.Success();
    }
    
    public Result SetPhoto(string photoUrl)
    {
        var photoResult = StaffPhoto.Create(photoUrl);
        if (photoResult.IsFailure)
            return photoResult;

        if (Photo.Equals(photoResult.Value))
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
        if (TimeSlots.Any(ts => ts.Id == id || ts.Date == date))
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

    public Result AddTimeSlotInterval(
        Guid timeSlotId,
        TimeSpan start,
        TimeSpan end)
    {
        var timeSlot = _timeSlots.FirstOrDefault(ts => ts.Id == timeSlotId);
        if (timeSlot == null)
            return Result.Failure(DomainErrors.TimeSlot.NotFound(timeSlotId));

        var addIntervalResult = timeSlot.AddInterval(start, end);
        return addIntervalResult.IsFailure
            ? addIntervalResult
            : Result.Success();
    }

    public Result UpdateTimeSlotIntervals(Guid timeSlotId, List<Interval> intervals)
    {
        var index = _timeSlots.FindIndex(ts => ts.Id == timeSlotId);
        if (index == -1)
            return Result.Failure(DomainErrors.TimeSlot.NotFound(timeSlotId));

        var updateIntervalsResult = _timeSlots[index].UpdateIntervals(intervals);
        if (updateIntervalsResult.IsFailure)
            return updateIntervalsResult;

        return Result.Success();
    }

    public Result AddService(Service service)
    {
        if (_services.Any(s => s.Id == service.Id))
            return Result.Success();

        _services.Add(service);
        return Result.Success();
    }

    public Result RemoveService(Service service)
    {
        var existingService = _services.FirstOrDefault(s => s.Id == service.Id);
        if (existingService is null)
            return Result.Success();

        _services.Remove(existingService);
        return Result.Success();
    }
}
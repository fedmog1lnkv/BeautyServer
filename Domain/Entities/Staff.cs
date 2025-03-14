using Domain.Enums;
using Domain.Errors;
using Domain.Primitives;
using Domain.Repositories.Organizations;
using Domain.Repositories.Staffs;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entities;

public class Staff : AggregateRoot
{
    private readonly List<Service> _services = [];
    private readonly List<TimeSlot> _timeSlots = [];

    private Staff(
        Guid id,
        Guid organizationId,
        StaffName name,
        StaffPhoneNumber phoneNumber,
        StaffRole role) : base(id)
    {
        OrganizationId = organizationId;
        Name = name;
        PhoneNumber = phoneNumber;
        Role = role;
    }

#pragma warning disable CS8618
    private Staff() { }
#pragma warning restore CS8618

    public Guid OrganizationId { get; private set; }
    public StaffName Name { get; private set; }
    public StaffPhoneNumber PhoneNumber { get; private set; }
    public StaffRole Role { get; private set; }
    public IReadOnlyCollection<Service> Services => _services.AsReadOnly();
    public IReadOnlyCollection<TimeSlot> TimeSlots => _timeSlots.AsReadOnly();

    public static async Task<Result<Staff>> Create(
        Guid id,
        Guid organizationId,
        string name,
        string phoneNumber,
        IStaffReadOnlyRepository staffRepository,
        IOrganizationReadOnlyRepository organizationRepository)
    {
        if (organizationId == Guid.Empty)
            return Result.Failure<Staff>(DomainErrors.Staff.OrganizationIdEmpty);

        var organizationExists = await organizationRepository.ExistsAsync(organizationId);
        if (!organizationExists)
            return Result.Failure<Staff>(DomainErrors.Organization.NotFound(organizationId));

        var staffPhoneResult = StaffPhoneNumber.Create(phoneNumber);
        if (staffPhoneResult.IsFailure)
            return Result.Failure<Staff>(staffPhoneResult.Error);

        var isUniquePhone = await staffRepository.IsPhoneNumberUnique(staffPhoneResult.Value);
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
            StaffRole.Unknown);
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

    public Result AddTimeSlot(TimeSlot timeSlot)
    {
        if (TimeSlots.Any(ts => ts.Id == timeSlot.Id))
            return Result.Success();

        _timeSlots.Add(timeSlot);
        return Result.Success();
    }

    public Result UpdateTimeSlot(TimeSlot updatedSlot)
    {
        var index = _timeSlots.FindIndex(ts => ts.Id == updatedSlot.Id);
        if (index == -1)
            return Result.Failure(DomainErrors.TimeSlot.NotFound(updatedSlot.Id));

        _timeSlots[index] = updatedSlot;
        return Result.Success();
    }
}
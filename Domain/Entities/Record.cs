using Domain.Enums;
using Domain.Errors;
using Domain.Primitives;
using Domain.Repositories.Organizations;
using Domain.Repositories.Services;
using Domain.Repositories.Staffs;
using Domain.Repositories.Users;
using Domain.Repositories.Venues;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entities;

public sealed class Record : AggregateRoot, IAuditableEntity
{
    private Record(
        Guid id,
        Guid userId,
        Guid staffId,
        Guid organizationId,
        Guid venueId,
        Guid serviceId,
        RecordStatus status,
        RecordComment? comment,
        DateTime startTimestamp,
        DateTime endTimestamp,
        DateTime createdOnUtc) : base(id)
    {
        UserId = userId;
        StaffId = staffId;
        OrganizationId = organizationId;
        VenueId = venueId;
        ServiceId = serviceId;
        Status = status;
        Comment = comment;
        StartTimestamp = startTimestamp;
        EndTimestamp = endTimestamp;

        CreatedOnUtc = createdOnUtc;
    }

#pragma warning disable CS8618
    private Record() { }
#pragma warning restore CS8618
    public Guid UserId { get; private set; }
    public Guid StaffId { get; private set; }
    public Guid OrganizationId { get; private set; }
    public Guid VenueId { get; private set; }
    public Guid ServiceId { get; private set; }
    public RecordStatus Status { get; private set; }
    public RecordComment? Comment { get; private set; }
    public DateTime StartTimestamp { get; private set; }
    public DateTime EndTimestamp { get; private set; }

    public User User { get; private set; } = null!;
    public Staff Staff { get; private set; } = null!;
    public Organization Organization { get; private set; } = null!;
    public Venue Venue { get; private set; } = null!;
    public Service Service { get; private set; } = null!;

    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    public static async Task<Result<Record>> CreateAsync(
        Guid id,
        Guid userId,
        Guid staffId,
        Guid organizationId,
        Guid venueId,
        Guid serviceId,
        RecordStatus status,
        DateTime startTimestamp,
        DateTime endTimestamp,
        DateTime createdOnUtc,
        IUserReadOnlyRepository userRepository,
        IStaffReadOnlyRepository staffRepository,
        IOrganizationReadOnlyRepository organizationRepository,
        IVenueReadOnlyRepository venueRepository,
        IServiceReadOnlyRepository serviceRepository,
        CancellationToken cancellationToken)
    {
        if (userId == Guid.Empty)
            return Result.Failure<Record>(DomainErrors.Record.UserIdEmpty);

        if (staffId == Guid.Empty)
            return Result.Failure<Record>(DomainErrors.Record.StaffIdEmpty);

        if (organizationId == Guid.Empty)
            return Result.Failure<Record>(DomainErrors.Record.OrganizationIdEmpty);

        if (venueId == Guid.Empty)
            return Result.Failure<Record>(DomainErrors.Record.VenueIdEmpty);

        if (serviceId == Guid.Empty)
            return Result.Failure<Record>(DomainErrors.Record.ServiceIdEmpty);

        var userExists = await userRepository.ExistsAsync(userId, cancellationToken);
        if (!userExists)
            return Result.Failure<Record>(DomainErrors.User.NotFound(userId));

        var staffExists = await staffRepository.ExistsAsync(staffId, cancellationToken);
        if (!staffExists)
            return Result.Failure<Record>(DomainErrors.Staff.NotFound(staffId));

        var organizationExists = await organizationRepository.ExistsAsync(organizationId, cancellationToken);
        if (!organizationExists)
            return Result.Failure<Record>(DomainErrors.Organization.NotFound(organizationId));

        var venueExists = await venueRepository.ExistsAsync(venueId, cancellationToken);
        if (!venueExists)
            return Result.Failure<Record>(DomainErrors.Venue.NotFound(venueId));

        var serviceExists = await serviceRepository.ExistsAsync(serviceId, cancellationToken);
        if (!serviceExists)
            return Result.Failure<Record>(DomainErrors.Service.NotFound(serviceId));

        var record = new Record(
            id,
            userId,
            staffId,
            organizationId,
            venueId,
            serviceId,
            status,
            null,
            startTimestamp,
            endTimestamp,
            createdOnUtc);

        return Result.Success(record);
    }

    public Result Approve(string? commentText)
    {
        var setCommentResult = SetComment(commentText!);
        if (setCommentResult.IsFailure)
            return setCommentResult;

        Status = RecordStatus.Approved;

        return Result.Success();
    }

    public Result Discard(string? commentText)
    {
        var setCommentResult = SetComment(commentText!);
        if (setCommentResult.IsFailure)
            return setCommentResult;

        Status = RecordStatus.Discarded;

        return Result.Success();
    }
    
    public Result Completed(string? commentText)
    {
        var setCommentResult = SetComment(commentText!);
        if (setCommentResult.IsFailure)
            return setCommentResult;

        Status = RecordStatus.Completed;

        return Result.Success();
    }

    public Result SetComment(string? commentText)
    {
        if (!string.IsNullOrEmpty(commentText))
            return Result.Failure(DomainErrors.RecordComment.Empty);

        var createCommentResult = RecordComment.Create(commentText!);
        if (createCommentResult.IsFailure)
            return createCommentResult;

        Comment = createCommentResult.Value;

        return Result.Success();
    }
    
    public Result SetTime(DateTime newStartTimestamp, DateTime newEndTimestamp)
    {
        if (newStartTimestamp >= newEndTimestamp)
            return Result.Failure(DomainErrors.TimeSlot.IntervalsOverlap);

        StartTimestamp = newStartTimestamp;
        EndTimestamp = newEndTimestamp;
        ModifiedOnUtc = DateTime.UtcNow;

        return Result.Success();
    }
}
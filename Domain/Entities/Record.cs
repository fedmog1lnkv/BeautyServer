using Domain.DomainEvents.Record;
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
    private readonly List<RecordStatusLog> _statusLogs = [];
    private readonly List<RecordMessage> _messages = [];

    private Record(
        Guid id,
        Guid userId,
        Guid staffId,
        Guid organizationId,
        Guid venueId,
        Guid serviceId,
        RecordStatus status,
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
        StartTimestamp = startTimestamp;
        EndTimestamp = endTimestamp;

        CreatedOnUtc = createdOnUtc;

        AddStatusLog(RecordStatusChange.Created, "Услуга создана.");
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
    public RecordReview? Review { get; private set; }
    public DateTimeOffset StartTimestamp { get; private set; }
    public DateTimeOffset EndTimestamp { get; private set; }

    public User User { get; private set; } = null!;
    public Staff Staff { get; private set; } = null!;
    public Organization Organization { get; private set; } = null!;
    public Venue Venue { get; private set; } = null!;
    public Service Service { get; private set; } = null!;
    public IReadOnlyCollection<RecordStatusLog> StatusLogs => _statusLogs.AsReadOnly();
    public IReadOnlyCollection<RecordMessage> Messages => _messages.AsReadOnly();

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
            startTimestamp,
            endTimestamp,
            createdOnUtc);

        return Result.Success(record);
    }

    public Result Approve()
    {
        Status = RecordStatus.Approved;

        AddStatusLog(RecordStatusChange.Approved, "Услуга одобрена.");

        return Result.Success();
    }

    public Result Discard()
    {
        Status = RecordStatus.Discarded;

        AddStatusLog(RecordStatusChange.Discarded, "Услуга отменена.");

        return Result.Success();
    }

    public Result Completed()
    {
        Status = RecordStatus.Completed;

        AddStatusLog(RecordStatusChange.Completed, "Услуга выполнена.");

        return Result.Success();
    }

    private void AddStatusLog(RecordStatusChange statusChange, string description)
    {
        var createLogResult = RecordStatusLog.Create(
            Guid.NewGuid(),
            Id,
            statusChange,
            description,
            DateTime.UtcNow);

        if (createLogResult.IsFailure)
            return;

        _statusLogs.Add(createLogResult.Value);
    }

    public Result SetReview(int rating, string? comment)
    {
        var reviewResult = RecordReview.Create(rating, comment);
        if (reviewResult.IsFailure)
            return reviewResult;

        Review = reviewResult.Value;

        AddStatusLog(RecordStatusChange.Review, $"Оставлен новый отзыв на {rating}\ud83c\udf1f");

        RaiseDomainEvent(
            new RecordReviewAddedChangedEvent(
                Guid.NewGuid(),
                Id,
                StaffId,
                ServiceId,
                VenueId,
                Review.Rating));

        return Result.Success();
    }

    public Result SetTime(DateTime newStartTimestamp, DateTime newEndTimestamp)
    {
        if (newStartTimestamp >= newEndTimestamp)
            return Result.Failure(DomainErrors.TimeSlot.IntervalsOverlap);

        StartTimestamp = newStartTimestamp;
        EndTimestamp = newEndTimestamp;
        ModifiedOnUtc = DateTime.UtcNow;

        AddStatusLog(RecordStatusChange.Moved, "Услуга перенесена");
        Status = RecordStatus.UserPending;

        return Result.Success();
    }

    public Result AddMessage(
        Guid senderId,
        string content,
        Guid? messageId)
    {
        var validMessageId = messageId.HasValue && messageId.Value != Guid.Empty
            ? messageId.Value
            : Guid.NewGuid();

        var createMessageResult = RecordMessage.Create(validMessageId, Id, senderId, content);

        if (createMessageResult.IsFailure)
            return createMessageResult;

        _messages.Add(createMessageResult.Value);

        return Result.Success();
    }

    public Result DeleteMessage(Guid senderId, Guid messageId)
    {
        var messageToDelete = _messages
            .FirstOrDefault(m => m.SenderId == senderId && m.Id == messageId);

        if (messageToDelete == null)
            return Result.Failure(DomainErrors.RecordChat.MessageNotFound(senderId, messageId));

        _messages.Remove(messageToDelete);

        return Result.Success();
    }

    public Result MarkMessageAsRead(Guid messageId, Guid readerId)
    {
        var message = _messages.FirstOrDefault(m => m.Id == messageId);
        if (message == null)
            return Result.Failure(DomainErrors.RecordMessage.NotFound(messageId));

        if (message.IsRead)
            return Result.Success();

        if (message.SenderId == readerId)
            return Result.Failure(DomainErrors.RecordMessage.CannotReadOwnMessage);

        var result = message.MarkAsRead(DateTime.UtcNow);
        if (result.IsFailure)
            return result;

        return Result.Success();
    }

    public int UnreadMessageCountStaff => _messages.Count(m => !m.IsRead && m.SenderId != StaffId);
    public int UnreadMessageCountUser => _messages.Count(m => !m.IsRead && m.SenderId != UserId);
}
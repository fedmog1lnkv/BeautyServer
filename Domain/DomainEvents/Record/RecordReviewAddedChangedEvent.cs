namespace Domain.DomainEvents.Record;

public record RecordReviewAddedChangedEvent(
    Guid Id,
    Guid RecordId,
    Guid StaffId,
    Guid ServiceId,
    Guid VenueId,
    int Rating) : DomainEvent(Id);
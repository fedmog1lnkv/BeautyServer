namespace Domain.DomainEvents.Record;

public record RecordReviewAddedChangedEvent(
    Guid Id,
    Guid StaffId,
    Guid ServiceId,
    Guid VenueId,
    byte Rating) : DomainEvent(Id);
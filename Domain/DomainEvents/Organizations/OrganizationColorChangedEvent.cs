namespace Domain.DomainEvents.Organizations;

public sealed record OrganizationColorChangedEvent(
    Guid Id,
    Guid OrganizationId,
    string OrganizationColorOld,
    string OrganizationColorNew) : DomainEvent(Id);
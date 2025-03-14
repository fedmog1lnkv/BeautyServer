namespace Domain.DomainEvents.Organizations;

public sealed record OrganizationPhotoChangedEvent(
    Guid Id,
    Guid OrganizationId,
    string? OrganizationPhotoOld,
    string OrganizationPhotoNew) : DomainEvent(Id);
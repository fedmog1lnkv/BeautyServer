namespace Application.Features.Organizations.Queries.GetOrganizationServicesById.Dto;

public sealed class ServiceLookupDto
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required int Rating { get; set; }
    public TimeSpan? Duration { get; set; }
    public double? Price { get; set; }
    public string? Photo { get; set; }
    public List<Guid> StaffIds { get; set; } = [];
    public List<Guid> VenueIds { get; set; } = [];
}
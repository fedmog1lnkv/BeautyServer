namespace Application.Features.Organizations.Queries.GetOrganizationStaffsById.Dto;

public sealed class StaffLookupDto
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Role { get; set; }
    public string? Photo { get; set; }
    public required int Rating { get; set; }
}
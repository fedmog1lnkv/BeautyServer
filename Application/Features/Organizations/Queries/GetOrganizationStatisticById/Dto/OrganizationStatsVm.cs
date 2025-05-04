namespace Application.Features.Organizations.Queries.GetOrganizationStatisticById.Dto;

public sealed class OrganizationStatsVm
{
    public int CompletedRecordsCount { get; set; }
    public int UniqueCustomersCount { get; set; }
    public int AverageRating { get; set; }
    public int TotalEarned { get; set; }
    
}
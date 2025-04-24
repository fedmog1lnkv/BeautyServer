namespace Application.Features.Venues.Queries.GetVenueClustersInBounds.Dto;

public class VenueClusterVm
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int Count { get; set; }
    public required List<Guid> VenueIds { get; set; }
}
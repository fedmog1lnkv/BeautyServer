using Domain.Entities;

namespace Application.Features.Venues.Queries.GetVenueClustersInBounds.Dto;

public class VenueClustersVm
{
    public List<VenueClusterVm> Clusters { get; set; } = [];
    public List<Venue> Venues { get; set; } = [];
}
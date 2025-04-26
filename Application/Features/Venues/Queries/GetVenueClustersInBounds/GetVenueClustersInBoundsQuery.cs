using Application.Features.Venues.Queries.GetVenueClustersInBounds.Dto;
using Application.Messaging.Query;
using Domain.Shared;

namespace Application.Features.Venues.Queries.GetVenueClustersInBounds;

public record GetVenueClustersInBoundsQuery(
    double MinLatitude,
    double MinLongitude,
    double MaxLatitude,
    double MaxLongitude,
    int Zoom) : IQuery<Result<VenueClustersVm>>;
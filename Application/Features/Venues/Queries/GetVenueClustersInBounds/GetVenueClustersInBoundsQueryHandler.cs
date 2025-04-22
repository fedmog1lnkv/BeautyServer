using Application.Features.Venues.Queries.GetVenueClustersInBounds.Dto;
using Application.Messaging.Query;
using Domain.Repositories.Venues;
using Domain.Shared;

namespace Application.Features.Venues.Queries.GetVenueClustersInBounds;

public sealed class GetVenueClustersInBoundsQueryHandler(IVenueReadOnlyRepository repository) :
    IQueryHandler<GetVenueClustersInBoundsQuery,
        Result<VenueClustersVm>>
{
    public async Task<Result<VenueClustersVm>> Handle(
        GetVenueClustersInBoundsQuery request,
        CancellationToken cancellationToken)
    {
        var venues = await repository.GetInBounds(
            request.MinLatitude,
            request.MinLongitude,
            request.MaxLatitude,
            request.MaxLongitude,
            cancellationToken);

        var vm = new VenueClustersVm();

        if (request.Zoom >= 15)
            vm.Venues = venues;
        else
        {
            double gridSize = GetGridSize(request.Zoom);

            var clusters = venues
                .GroupBy(
                    v => new
                    {
                        LatKey = Math.Floor(v.Location.Latitude / gridSize),
                        LngKey = Math.Floor(v.Location.Longitude / gridSize)
                    })
                .Select(
                    g => new VenueClusterVm
                    {
                        Latitude = g.Average(v => v.Location.Latitude),
                        Longitude = g.Average(v => v.Location.Longitude),
                        Count = g.Count()
                    })
                .ToList();

            vm.Clusters = clusters;
        }

        return Result.Success(vm);
    }

    private double GetGridSize(int zoom)
    {
        return zoom switch
        {
            <= 5 => 1.0,
            <= 8 => 0.5,
            <= 10 => 0.2,
            <= 12 => 0.1,
            <= 14 => 0.05,
            _ => 0.01
        };
    }
}
using Application.Features.Venues.Queries.GetVenueClustersInBounds.Dto;
using Application.Messaging.Query;
using Domain.Entities;
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
            request.Search,
            cancellationToken);

        var vm = new VenueClustersVm();

        if (request.Zoom >= 15)
        {
            vm.Venues = venues;
        }
        else
        {
            double gridSize = GetGridSize(request.Zoom);

            var clustersOrSingles = venues
                .GroupBy(
                    v => new
                    {
                        LatKey = Math.Floor(v.Location.Latitude / gridSize),
                        LngKey = Math.Floor(v.Location.Longitude / gridSize)
                    });

            var clusters = new List<VenueClusterVm>();
            var singles = new List<Venue>();

            foreach (var group in clustersOrSingles)
            {
                if (group.Count() == 1)
                {
                    singles.Add(group.First());
                }
                else
                {
                    clusters.Add(
                        new VenueClusterVm
                        {
                            Latitude = group.Average(v => v.Location.Latitude),
                            Longitude = group.Average(v => v.Location.Longitude),
                            Count = group.Count(),
                            VenueIds = group.Select(v => v.Id).ToList()
                        });
                }
            }

            vm.Venues = singles;
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
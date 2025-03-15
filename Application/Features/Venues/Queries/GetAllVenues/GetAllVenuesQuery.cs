using Application.Messaging.Query;
using Domain.Entities;
using Domain.Shared;

namespace Application.Features.Venues.Queries.GetAllVenues;

public record GetAllVenuesQuery(
    int Limit,
    int Offset,
    double? Latitude,
    double? Longitude) : IQuery<Result<List<Venue>>>;
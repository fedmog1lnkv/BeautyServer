using Application.Messaging.Query;
using Domain.Entities;
using Domain.Shared;

namespace Application.Features.Venues.Queries.GetVenueById;

public record GetVenueByIdQuery(Guid Id) : IQuery<Result<Venue>>;
using Application.Messaging.Query;
using Domain.Entities;
using Domain.Shared;

namespace Application.Features.Services.Queries.GetServicesByVenueId;

public record GetServicesByVenueIdQuery(Guid VenueId) : IQuery<Result<List<Service>>>;
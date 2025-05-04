using Application.Messaging.Query;
using Domain.Entities;
using Domain.Shared;

namespace Application.Features.Organizations.Queries.GetOrganiazationVenuesById;

public record GetOrganizationVenuesByIdQuery(Guid Id) : IQuery<Result<List<Venue>>>;
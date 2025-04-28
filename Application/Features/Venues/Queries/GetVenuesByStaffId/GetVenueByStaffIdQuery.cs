using Application.Messaging.Query;
using Domain.Entities;
using Domain.Shared;

namespace Application.Features.Venues.Queries.GetVenuesByStaffId;

public record GetVenuesByStaffIdQuery(Guid StaffId) : IQuery<Result<List<Venue>>>;
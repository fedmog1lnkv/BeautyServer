using Application.Messaging.Query;
using Domain.Entities;
using Domain.Shared;

namespace Application.Features.Staffs.Queries.GetStaffWithServicesByVenueId;

public record GetStaffWithServicesByVenueIdQuery(Guid VenueId) : IQuery<Result<List<Staff>>>;
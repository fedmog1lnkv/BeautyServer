using Application.Messaging.Query;
using Domain.Entities;
using Domain.Shared;

namespace Application.Features.Staffs.Queries.GetStaffWithTimeSlotsByIdAndVenueId;

public record GetStaffWithTimeSlotsByIdAndVenueIdQuery(Guid StaffId, Guid VenueId) : IQuery<Result<Staff>>;
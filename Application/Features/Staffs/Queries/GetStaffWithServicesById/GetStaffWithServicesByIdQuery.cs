using Application.Messaging.Query;
using Domain.Entities;
using Domain.Shared;

namespace Application.Features.Staffs.Queries.GetStaffWithServicesById;

public record GetStaffWithServicesByIdQuery(Guid Id) : IQuery<Result<Staff>>;
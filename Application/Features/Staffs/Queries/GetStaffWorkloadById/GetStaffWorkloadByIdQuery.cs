using Application.Features.Staffs.Queries.GetStaffWorkloadById.Dto;
using Application.Messaging.Query;
using Domain.Shared;

namespace Application.Features.Staffs.Queries.GetStaffWorkloadById;

public record GetStaffWorkloadByIdQuery(
    Guid StaffId,
    int Year,
    int Month) : IQuery<Result<StaffWorkloadCalendarVm>>;
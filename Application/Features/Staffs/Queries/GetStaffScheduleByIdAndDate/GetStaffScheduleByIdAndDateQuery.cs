using Application.Features.Staffs.Queries.GetStaffScheduleByIdAndDate.Dto;
using Application.Messaging.Query;
using Domain.Shared;

namespace Application.Features.Staffs.Queries.GetStaffScheduleByIdAndDate;

public record GetStaffScheduleByIdAndDateQuery(
    Guid StaffId,
    int Year,
    int Month,
    int Day) :
    IQuery<Result<List<StaffScheduleForDayVm>>>;
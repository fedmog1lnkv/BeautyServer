using Application.Features.Staffs.Queries.GetStaffWorkloadById.Dto;
using Application.Messaging.Query;
using Domain.Enums;
using Domain.Errors;
using Domain.Repositories.Records;
using Domain.Repositories.Staffs;
using Domain.Shared;

namespace Application.Features.Staffs.Queries.GetStaffWorkloadById;

public sealed class GetStaffWorkloadByIdQueryHandler(
    IRecordReadOnlyRepository recordReadOnlyRepository,
    IStaffReadOnlyRepository staffReadOnlyRepository) :
    IQueryHandler<GetStaffWorkloadByIdQuery,
        Result<StaffWorkloadCalendarVm>>
{
    public async Task<Result<StaffWorkloadCalendarVm>> Handle(
        GetStaffWorkloadByIdQuery request,
        CancellationToken cancellationToken)
    {
        var isStaffExists = await staffReadOnlyRepository.ExistsAsync(request.StaffId, cancellationToken);
        if (!isStaffExists)
            return Result.Failure<StaffWorkloadCalendarVm>(DomainErrors.Staff.NotFound(request.StaffId));

        var records = await recordReadOnlyRepository.GetByStaffIdAndMonth(
            request.StaffId,
            request.Year,
            request.Month,
            cancellationToken);
        var recordsByDate = records.GroupBy(r => r.StartTimestamp.Date).OrderBy(g => g.Key).ToDictionary(
            g => g.Key,
            g
                => g.ToList());
        var daysInMonth = DateTime.DaysInMonth(request.Year, request.Month);

        var response = new StaffWorkloadCalendarVm();
        for (var day = 1; day <= daysInMonth; day++)
        {
            var currentDate = new DateTime(request.Year, request.Month, day);
            recordsByDate.TryGetValue(currentDate, out var dayRecords);

            string status;

            if (dayRecords is null || dayRecords.Count == 0)
                status = "noOrders";
            else if (dayRecords.Any(r => r.Status == RecordStatus.Pending))
                status = "newOrders";
            else if (dayRecords.Any(r => r.Status == RecordStatus.Approved))
                status = "approved";
            else if (dayRecords.All(
                         r =>
                             r.Status is RecordStatus.Completed or RecordStatus.Discarded))
                status = "completed";
            else
                status = "noOrders";

            response.Days.Add(
                new StaffWorkloadDay
                {
                    Day = day,
                    Status = status
                });
        }

        return Result.Success(response);
    }
}
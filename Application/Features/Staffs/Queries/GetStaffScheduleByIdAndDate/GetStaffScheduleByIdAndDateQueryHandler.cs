using Application.Features.Staffs.Queries.GetStaffScheduleByIdAndDate.Dto;
using Application.Messaging.Query;
using Domain.Enums;
using Domain.Errors;
using Domain.Repositories.Records;
using Domain.Repositories.Staffs;
using Domain.Shared;
using System.Globalization;

namespace Application.Features.Staffs.Queries.GetStaffScheduleByIdAndDate;

public class GetStaffScheduleByIdAndDateQueryHandler(
    IRecordReadOnlyRepository recordReadOnlyRepository,
    IStaffReadOnlyRepository staffReadOnlyRepository) : IQueryHandler<GetStaffScheduleByIdAndDateQuery,
    Result<StaffScheduleForDayVm>>
{
    public async Task<Result<StaffScheduleForDayVm>> Handle(
        GetStaffScheduleByIdAndDateQuery request,
        CancellationToken cancellationToken)
    {
        var staff = await staffReadOnlyRepository.GetByIdWithTimeSlots(request.StaffId, cancellationToken);
        if (staff is null)
            return Result.Failure<StaffScheduleForDayVm>(DomainErrors.Staff.NotFound(request.StaffId));

        var dateOnly = new DateOnly(request.Year, request.Month, request.Day);

        var records = await recordReadOnlyRepository.GetByStaffIdAndDateAsync(
            request.StaffId,
            dateOnly,
            int.MaxValue,
            0,
            false,
            cancellationToken);
        var timeSlot = staff.TimeSlots.FirstOrDefault(ts => ts.Date == dateOnly);

        var response = new StaffScheduleForDayVm();

        if (timeSlot is null && records.Count == 0)
            return Result.Success(response);

        response.TimeSlotId = timeSlot!.Id;

        response.Workload.AddRange(
            records
                .Where(r => r.Status != RecordStatus.Discarded)
                .Select(
                    record => new WorkloadTimeSlotDto
                    {
                        Type = WorkloadTimeSlotType.Record.ToString(),
                        RecordInfo = new RecordInfo
                        {
                            Id = record.Id,
                            Status = record.Status.ToString(),
                            ClientName = record.User.Name.Value,
                            ServiceName = record.Service.Name.Value
                        },
                        Interval = new TimeIntervalDto
                        {
                            Start = TimeZoneInfo.ConvertTimeFromUtc(
                                record.StartTimestamp.DateTime, record.Venue.TimeZone).TimeOfDay.ToString(),
                            End = TimeZoneInfo.ConvertTimeFromUtc(record.EndTimestamp.DateTime, record.Venue.TimeZone)
                                .TimeOfDay.ToString()
                        }
                    }));

        response.Workload.AddRange(
            timeSlot.Intervals.Select(
                freeTime => new WorkloadTimeSlotDto
                {
                    Type = WorkloadTimeSlotType.FreeTime.ToString(),
                    RecordInfo = null,
                    Interval = new TimeIntervalDto
                    {
                        Start = freeTime.Start.ToString(),
                        End = freeTime.End.ToString()
                    }
                }));

        return Result.Success(response);
    }
}
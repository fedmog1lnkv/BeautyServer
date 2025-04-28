using Application.Features.Staffs.Queries.GetStaffScheduleByIdAndDate.Dto;
using Application.Messaging.Query;
using Domain.Enums;
using Domain.Errors;
using Domain.Repositories.Records;
using Domain.Repositories.Staffs;
using Domain.Repositories.Venues;
using Domain.Shared;
using System.Globalization;

namespace Application.Features.Staffs.Queries.GetStaffScheduleByIdAndDate;

public class GetStaffScheduleByIdAndDateQueryHandler(
    IRecordReadOnlyRepository recordReadOnlyRepository,
    IStaffReadOnlyRepository staffReadOnlyRepository,
    IVenueReadOnlyRepository venueReadOnlyRepository) : IQueryHandler<GetStaffScheduleByIdAndDateQuery,
    Result<List<StaffScheduleForDayVm>>>
{
    public async Task<Result<List<StaffScheduleForDayVm>>> Handle(
        GetStaffScheduleByIdAndDateQuery request,
        CancellationToken cancellationToken)
    {
        var staff = await staffReadOnlyRepository.GetByIdWithTimeSlots(request.StaffId, cancellationToken);
        if (staff is null)
            return Result.Failure<List<StaffScheduleForDayVm>>(DomainErrors.Staff.NotFound(request.StaffId));

        var dateOnly = new DateOnly(request.Year, request.Month, request.Day);

        var records = await recordReadOnlyRepository.GetByStaffIdAndDateAsync(
            request.StaffId,
            dateOnly,
            int.MaxValue,
            0,
            false,
            cancellationToken);
        var timeSlots = staff.TimeSlots.Where(ts => ts.Date == dateOnly).ToList();

        var response = new StaffScheduleForTimeSlotVm();

        if (timeSlots.Count == 0 && records.Count == 0)
            return Result.Success(response.TimeSlots);

        foreach (var timeSlot in timeSlots)
        {
            var venue = await venueReadOnlyRepository.GetByIdAsync(timeSlot.VenueId, cancellationToken);
            var daySchedule = new StaffScheduleForDayVm
                {
                    TimeSlotId = timeSlot.Id,
                    VenueId = timeSlot.VenueId,
                    VenueName = venue!.Name.Value,
                    Workload = []
                };

                // Добавляем свободное время (Intervals)
                daySchedule.Workload.AddRange(timeSlot.Intervals.Select(freeTime => new WorkloadTimeSlotDto
                {
                    Type = WorkloadTimeSlotType.FreeTime.ToString(),
                    RecordInfo = null,
                    Interval = new TimeIntervalDto
                    {
                        Start = freeTime.Start.ToString(),
                        End = freeTime.End.ToString()
                    }
                }));

                // Добавляем записи (Records)
                daySchedule.Workload.AddRange(records.Where(r => r.Status != RecordStatus.Discarded).Select(record => new WorkloadTimeSlotDto
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

                response.TimeSlots.Add(daySchedule);
        }

        return Result.Success(response.TimeSlots);
    }
}
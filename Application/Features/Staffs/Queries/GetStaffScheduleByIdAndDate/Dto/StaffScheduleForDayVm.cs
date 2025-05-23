namespace Application.Features.Staffs.Queries.GetStaffScheduleByIdAndDate.Dto;

public class StaffScheduleForTimeSlotVm
{
    public List<StaffScheduleForDayVm> TimeSlots { get; set; } = [];
}
public class StaffScheduleForDayVm
{
    public Guid TimeSlotId { get; set; }
    public Guid VenueId { get; set; }
    public required string VenueName { get; set; }
    public List<WorkloadTimeSlotDto> Workload { get; set; } = [];
}

public class WorkloadTimeSlotDto
{
    public required string Type { get; set; }
    public RecordInfo? RecordInfo { get; set; }
    public required TimeIntervalDto Interval { get; set; }
}

public class RecordInfo
{
    public Guid Id { get; set; }
    public required string Status { get; set; }
    public required int UnreadMessageCount { get; set; }
    public required string ClientName { get; set; }
    public required string ServiceName { get; set; }
}

public class TimeIntervalDto
{
    public required string Start { get; set; }
    public required string End { get; set; }
}

public enum WorkloadTimeSlotType
{
    FreeTime,
    Record
}
namespace Application.Features.Staffs.Queries.GetStaffScheduleByIdAndDate.Dto;

public class StaffScheduleForDayVm
{
    public Guid? TimeSlotId { get; set; }
    public List<WorkloadTimeSlotDto> Workload { get; set; } = [];
}

public class WorkloadTimeSlotDto
{
    public required string Type { get; set; }
    public Guid? RecordId { get; set; }
    public required TimeIntervalDto Interval { get; set; }
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
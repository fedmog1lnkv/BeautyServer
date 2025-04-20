namespace Application.Features.Staffs.Queries.GetStaffWorkloadById.Dto;

public class StaffWorkloadCalendarVm
{
    public List<StaffWorkloadDay> Days { get; set; } = [];
}

public class StaffWorkloadDay
{
    public int Day { get; set; }
    public string Status { get; set; }
}
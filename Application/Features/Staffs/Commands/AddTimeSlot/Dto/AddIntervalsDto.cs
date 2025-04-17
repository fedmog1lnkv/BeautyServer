namespace Application.Features.Staffs.Commands.AddTimeSlot.Dto;

public class AddIntervalsDto
{
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}
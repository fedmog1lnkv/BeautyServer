using Application.Features.Staffs.Commands.UpdateTimeSlot.Dto;
using Application.Messaging.Command;
using Domain.Shared;

namespace Application.Features.Staffs.Commands.UpdateTimeSlot;

public record UpdateTimeSlotCommand(
    Guid InitiatorId,
    Guid StaffId,
    Guid TimeSlotId,
    List<UpdateIntervalsDto> Intervals) : ICommand<Result>;
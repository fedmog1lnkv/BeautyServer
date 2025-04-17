using Application.Features.Staffs.Commands.AddTimeSlot.Dto;
using Application.Messaging.Command;
using Domain.Shared;

namespace Application.Features.Staffs.Commands.AddTimeSlot;

public record AddTimeSlotCommand(
    Guid InitiatorId,
    Guid StaffId,
    Guid VenueId,
    DateOnly Date,
    List<AddIntervalsDto> Intervals) : ICommand<Result>;
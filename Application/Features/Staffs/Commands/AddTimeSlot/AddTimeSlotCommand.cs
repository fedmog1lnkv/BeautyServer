using Application.Features.Staffs.Commands.AddTimeSlot.Dto;
using Application.Messaging.Command;
using Domain.Shared;

namespace Application.Features.Staffs.Commands.AddTimeSlot;

public record AddTimeSlotCommand(
    Guid StaffId,
    Guid VenueId,
    DateOnly Date,
    List<IntervalsDto> Intervals) : ICommand<Result>;
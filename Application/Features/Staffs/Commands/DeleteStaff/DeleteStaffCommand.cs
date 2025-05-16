using Application.Messaging.Command;
using Domain.Shared;

namespace Application.Features.Staffs.Commands.DeleteStaff;

public record DeleteStaffCommand(Guid InitiatorId, Guid StaffId) : ICommand<Result>;
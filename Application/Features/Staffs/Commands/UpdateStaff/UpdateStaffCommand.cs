using Application.Messaging.Command;
using Domain.Shared;

namespace Application.Features.Staffs.Commands.UpdateStaff;

public record UpdateStaffCommand(
    Guid
        InitiatorId, // Тот кто инициировал запрос на обновление данных о персонале (может делать либо менеджер, либо сам персонал)
    Guid StaffId,
    string? Name,
    string? Photo,
    List<Guid>? ServiceIds) : ICommand<Result>;
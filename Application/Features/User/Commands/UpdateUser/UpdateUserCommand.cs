using Application.Messaging.Command;
using Domain.Shared;

namespace Application.Features.User.Commands.UpdateUser;

public record UpdateUserCommand(
    Guid Id,
    string? Name,
    string? FirebaseToken,
    bool? ReceiveOrderNotifications,
    bool? ReceivePromoNotifications) : ICommand<Result>;
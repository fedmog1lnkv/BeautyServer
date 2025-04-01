using Application.Messaging.Command;
using Domain.Shared;

namespace Application.Features.User.Commands.UpdateUser;

public record UpdateUserCommand(Guid Id, string? Name) : ICommand<Result>;
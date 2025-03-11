using Application.Messaging.Command;
using Domain.Shared;

namespace Application.Features.Authorization.Commands.Register;

public record RegisterCommand(string Username, string Password) : ICommand<Result<string>>;
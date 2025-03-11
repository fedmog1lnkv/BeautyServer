using Application.Features.User.Commands.Auth.Dto;
using Application.Messaging.Command;
using Domain.Shared;

namespace Application.Features.User.Commands.Auth;

public record AuthCommand(string PhoneNumber, string Code) : ICommand<Result<AuthVm>>;
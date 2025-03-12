using Application.Features.User.Commands.RefreshToken.Dto;
using Application.Messaging.Command;
using Domain.Shared;

namespace Application.Features.User.Commands.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : ICommand<Result<TokensVm>>;
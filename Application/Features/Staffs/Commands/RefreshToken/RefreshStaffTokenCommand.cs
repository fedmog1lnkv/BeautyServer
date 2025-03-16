using Application.Features.Staffs.Commands.RefreshToken.Dto;
using Application.Messaging.Command;
using Domain.Shared;

namespace Application.Features.Staffs.Commands.RefreshToken;

public record RefreshStaffTokenCommand(string RefreshToken) : ICommand<Result<TokensVm>>;
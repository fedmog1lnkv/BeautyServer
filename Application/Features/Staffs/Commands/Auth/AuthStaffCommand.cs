using Application.Features.Staffs.Commands.Auth.Dto;
using Application.Messaging.Command;
using Domain.Shared;

namespace Application.Features.Staffs.Commands.Auth;

public record AuthStaffCommand(string PhoneNumber, string Code) : ICommand<Result<AuthVm>>;
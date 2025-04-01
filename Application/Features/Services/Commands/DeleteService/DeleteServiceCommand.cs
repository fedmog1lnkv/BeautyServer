using Application.Messaging.Command;
using Domain.Shared;

namespace Application.Features.Services.Commands.DeleteService;

public record DeleteServiceCommand(Guid Id) : ICommand<Result>;
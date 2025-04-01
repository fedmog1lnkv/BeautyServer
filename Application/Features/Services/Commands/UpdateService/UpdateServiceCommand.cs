using Application.Messaging.Command;
using Domain.Shared;

namespace Application.Features.Services.Commands.UpdateService;

public record UpdateServiceCommand(
    Guid Id,
    string? Name,
    string? Description,
    int? Duration,
    double? Price) : ICommand<Result>;
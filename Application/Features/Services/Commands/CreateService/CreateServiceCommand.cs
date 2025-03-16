using Application.Messaging.Command;
using Domain.Shared;

namespace Application.Features.Services.Commands.CreateService;

public record CreateServiceCommand(
    Guid OrganizationId,
    string Name,
    string? Description,
    int? Duration,
    double? Price) : ICommand<Result>;
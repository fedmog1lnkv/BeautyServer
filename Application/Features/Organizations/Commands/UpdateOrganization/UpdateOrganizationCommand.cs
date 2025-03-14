using Application.Messaging.Command;
using Domain.Shared;

namespace Application.Features.Organizations.Commands.UpdateOrganization;

public record UpdateOrganizationCommand(
    Guid Id,
    string? Name,
    string? Description,
    string? Subscription,
    string? Color,
    string? Photo) : ICommand<Result>;
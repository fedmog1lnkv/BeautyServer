using Application.Messaging.Command;
using Domain.Shared;

namespace Application.Features.Organizations.Commands.CreateOrganization;

public record CreateOrganizationCommand(
    Guid Id,
    string Name,
    string? Description,
    string? Color,
    string? Photo) : ICommand<Result>;
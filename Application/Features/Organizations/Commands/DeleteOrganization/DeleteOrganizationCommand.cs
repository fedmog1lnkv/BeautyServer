using Application.Messaging.Command;
using Domain.Shared;

namespace Application.Features.Organizations.Commands.DeleteOrganization;

public record DeleteOrganizationCommand(Guid Id) : ICommand<Result>;
using Application.Abstractions;
using Application.Features.Organizations.Commands.UpdateOrganization;
using Application.Messaging.Command;
using Domain.DomainEvents.Organizations;
using Domain.Errors;
using Domain.Extensions;
using Domain.Repositories.Organizations;
using Domain.Shared;

namespace Application.Features.Organizations.Commands.DeleteOrganization;

public class DeleteOrganizationCommandHandler(IOrganizationRepository organizationRepository, IDomainEventBus eventBus)
    : ICommandHandler<DeleteOrganizationCommand, Result>
{
    public async Task<Result> Handle(DeleteOrganizationCommand request, CancellationToken cancellationToken)
    {
        var organization = await organizationRepository.GetByIdAsync(request.Id, cancellationToken);

        if (organization is null)
            return Result.Failure(DomainErrors.Organization.NotFound(request.Id));
        
        organizationRepository.Remove(organization);

        return Result.Success();
    }
}
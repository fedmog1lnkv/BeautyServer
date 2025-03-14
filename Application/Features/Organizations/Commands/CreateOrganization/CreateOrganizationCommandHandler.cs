using Application.Messaging.Command;
using Domain.Entities;
using Domain.Repositories.Organizations;
using Domain.Shared;

namespace Application.Features.Organizations.Commands.CreateOrganization;

internal sealed class CreateOrganizationCommandHandler(IOrganizationRepository organizationRepository)
    : ICommandHandler<CreateOrganizationCommand, Result>
{
    // TODO : GET FROM APPSETTINGS
    private readonly string _defaultColor = "#FFFFFF";

    public async Task<Result> Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
    {
        var orgColor = request.Color ?? _defaultColor;

        var createOrganizationResult = Organization.Create(
            Guid.NewGuid(),
            request.Name,
            orgColor);

        if (createOrganizationResult.IsFailure)
            return Result.Failure(createOrganizationResult.Error);

        var organization = createOrganizationResult.Value;

        if (request.Photo is not null)
        {
            var result = organization.SetPhoto(request.Photo);
            if (result.IsFailure)
                return Result.Failure(result.Error);
        }

        if (request.Description is not null)
        {
            var result = organization.SetDescription(request.Description);
            if (result.IsFailure)
                return Result.Failure(result.Error);
        }

        organizationRepository.Add(organization);

        return Result.Success();
    }
}
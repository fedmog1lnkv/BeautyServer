using Application.Abstractions;
using Application.Messaging.Command;
using Domain.DomainEvents.Organizations;
using Domain.Errors;
using Domain.Extensions;
using Domain.Repositories.Organizations;
using Domain.Shared;

namespace Application.Features.Organizations.Commands.UpdateOrganization;

public sealed class UpdateOrganizationCommandHandler(IOrganizationRepository organizationRepository, IDomainEventBus eventBus)
    : ICommandHandler<UpdateOrganizationCommand, Result>
{
    public async Task<Result> Handle(UpdateOrganizationCommand request, CancellationToken cancellationToken)
    {
        var organization = await organizationRepository.GetByIdAsync(request.Id, cancellationToken);

        if (organization is null)
            return Result.Failure(DomainErrors.Organization.NotFound(request.Id));


        var result = Result.Success();

        if (request.Name != null)
        {
            result = organization.SetName(request.Name);
            if (result.IsFailure)
                return result;
        }

        if (request.Description != null)
        {
            result = organization.SetDescription(request.Description);
            if (result.IsFailure)
                return result;
        }

        if (!string.IsNullOrEmpty(request.Subscription))
        {
            var subscriptionResult = request.Subscription.ToEnum();
            if (subscriptionResult.IsFailure)
                return subscriptionResult;

            result = organization.SetSubscription(subscriptionResult.Value);
            if (result.IsFailure)
                return result;
        }

        if (request.Color != null)
        {
            var organizationColorOld = organization.Theme.Color;
            result = organization.SetColor(request.Color);
            if (result.IsFailure)
                return result;

            await eventBus.SendAsync(
                new OrganizationColorChangedEvent(
                    Guid.NewGuid(),
                    organization.Id,
                    organizationColorOld,
                    organization.Theme.Color),
                cancellationToken);
        }
        
        if (request.Photo is not null)
        {
            var isBase64 = request.Photo.Length % 4 == 0 &&
                           request.Photo.Replace(" ", "").Replace("\n", "").Replace("\r", "").All(
                               c => char.IsLetterOrDigit(c) || c == '+' || c == '/' || c == '=');

            var photoUrl = isBase64
                ? await organizationRepository.UploadPhotoAsync(request.Photo, organization.Id.ToString())
                : request.Photo;

            if (string.IsNullOrEmpty(photoUrl))
                return Result.Failure(DomainErrors.Organization.PhotoUploadFailed);

            var organizationPhotoOld = organization.Theme.Photo;
            result = organization.SetPhoto(photoUrl);
    
            if (result.IsFailure)
                return Result.Failure(result.Error);

            await eventBus.SendAsync(
                new OrganizationPhotoChangedEvent(
                    Guid.NewGuid(),
                    organization.Id,
                    organizationPhotoOld,
                    organization.Theme.Photo!),
                cancellationToken);
            
            if (organizationPhotoOld is not null)
                await organizationRepository.DeletePhoto(organizationPhotoOld);
        }


        // await integrationEventBus.SendAsync(
        //     new OrganizationUpdatedIntegrationEvent(
        //         Guid.NewGuid(),
        //         organization.Id),
        //     cancellationToken);

        return result;
    }
}
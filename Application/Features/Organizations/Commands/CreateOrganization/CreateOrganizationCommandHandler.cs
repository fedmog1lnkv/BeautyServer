using Application.Messaging.Command;
using Domain.Entities;
using Domain.Errors;
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
            orgColor,
            DateTime.UtcNow);

        if (createOrganizationResult.IsFailure)
            return Result.Failure(createOrganizationResult.Error);

        var organization = createOrganizationResult.Value;

        if (request.Description is not null)
        {
            var result = organization.SetDescription(request.Description);
            if (result.IsFailure)
                return Result.Failure(result.Error);
        }

        if (request.Photo is not null)
        {
            var isBase64 = request.Photo.Length % 4 == 0 &&
                           request.Photo.Replace(" ", "").Replace("\n", "").Replace("\r", "").All(
                               c => char.IsLetterOrDigit(c) || c == '+' || c == '/' || c == '=');

            var oldPhotoUrl = organization.Theme.Photo;
            var photoId = Guid.NewGuid();

            var photoUrl = isBase64
                ? await organizationRepository.UploadPhotoAsync(request.Photo, photoId.ToString())
                : request.Photo;

            if (string.IsNullOrEmpty(photoUrl))
                return Result.Failure(DomainErrors.Organization.PhotoUploadFailed);

            var result = organization.SetPhoto(photoUrl);
            if (result.IsFailure)
                return Result.Failure(result.Error);

            if (oldPhotoUrl is not null)
                await organizationRepository.DeletePhoto(oldPhotoUrl);
        }
        
        organizationRepository.Add(organization);

        return Result.Success();
    }
}
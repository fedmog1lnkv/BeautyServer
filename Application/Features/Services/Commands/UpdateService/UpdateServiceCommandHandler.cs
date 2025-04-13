using Application.Messaging.Command;
using Domain.Errors;
using Domain.Repositories.Services;
using Domain.Shared;

namespace Application.Features.Services.Commands.UpdateService;

public sealed class UpdateServiceCommandHandler(IServiceRepository serviceRepository)
    : ICommandHandler<UpdateServiceCommand, Result>
{
    public async Task<Result> Handle(UpdateServiceCommand request, CancellationToken cancellationToken)
    {
        var service = await serviceRepository.GetById(request.Id, cancellationToken);

        if (service is null)
            return Result.Failure(DomainErrors.Service.NotFound(request.Id));

        var result = Result.Success();

        if (request.Name != null)
        {
            result = service.SetName(request.Name);
            if (result.IsFailure)
                return result;
        }

        if (request.Description != null)
        {
            result = service.SetDescription(request.Description);
            if (result.IsFailure)
                return result;
        }

        if (request.Duration.HasValue)
        {
            result = service.SetDuration(request.Duration.Value);
            if (result.IsFailure)
                return result;
        }

        if (request.Price.HasValue)
        {
            result = service.SetPrice(request.Price.Value);
            if (result.IsFailure)
                return result;
        }
        
        if (request.Photo != null)
        {
            var photoUrl = await serviceRepository.UploadPhotoAsync(request.Photo, service.Id.ToString());

            if (string.IsNullOrEmpty(photoUrl))
                return Result.Failure(DomainErrors.Staff.PhotoUploadFailed);

            var setPhotoResult = service.SetPhoto(photoUrl);
            if (setPhotoResult.IsFailure)
                return setPhotoResult;
        }

        return result;
    }
}
using Application.Messaging.Command;
using Domain.Errors;
using Domain.Repositories.Users;
using Domain.Shared;

namespace Application.Features.User.Commands.UpdateUser;

public sealed class UpdateUserCommandHandler(IUserRepository userRepository)
    : ICommandHandler<UpdateUserCommand, Result>
{
    public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken);

        if (user is null)
            return Result.Failure(DomainErrors.User.NotFound(request.Id));

        var result = Result.Success();

        if (request.Name != null)
        {
            result = user.SetName(request.Name);
            if (result.IsFailure)
                return result;
        }

        if (request.FirebaseToken != null)
        {
            result = user.SetFirebaseToken(request.FirebaseToken);
            if (result.IsFailure)
                return result;
        }

        if (request.ReceiveOrderNotifications.HasValue || request.ReceivePromoNotifications.HasValue)
        {
            result = user.SetNotificationPreferences(
                request.ReceiveOrderNotifications,
                request.ReceivePromoNotifications);
            if (result.IsFailure)
                return result;
        }
        
        if (request.Photo != null)
        {
            var photoUrl = await userRepository.UploadPhotoAsync(request.Photo, user.Id.ToString());

            if (string.IsNullOrEmpty(photoUrl))
                return Result.Failure(DomainErrors.Staff.PhotoUploadFailed);

            var setPhotoResult = user.SetPhoto(photoUrl);
            if (setPhotoResult.IsFailure)
                return setPhotoResult;
        }

        return result;
    }
}
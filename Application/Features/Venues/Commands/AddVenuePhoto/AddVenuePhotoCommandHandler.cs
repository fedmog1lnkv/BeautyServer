using Application.Messaging.Command;
using Domain.Enums;
using Domain.Errors;
using Domain.Repositories.Staffs;
using Domain.Repositories.Venues;
using Domain.Shared;

namespace Application.Features.Venues.Commands.AddVenuePhoto;

public class AddVenuePhotoCommandHandler(
    IVenueRepository venueRepository,
    IStaffReadOnlyRepository staffReadOnlyRepository) :
    ICommandHandler<AddVenuePhotoCommand, Result>
{
    public async Task<Result> Handle(AddVenuePhotoCommand request, CancellationToken cancellationToken)
    {
        var staff = await staffReadOnlyRepository.GetByIdAsync(request.StaffId, cancellationToken);
        if (staff is null)
            return Result.Failure(DomainErrors.Staff.NotFound(request.StaffId));
        
        var venue = await venueRepository.GetByIdWithPhotos(request.VenueId, cancellationToken);
        if (venue is null)
            return Result.Failure(DomainErrors.Venue.NotFound(request.VenueId));
        
        if (staff.Role != StaffRole.Manager || staff.OrganizationId != venue.OrganizationId)
            return Result.Failure(DomainErrors.Staff.CannotUpdate);

        var photoId = Guid.NewGuid();

        var photoUrl = await venueRepository.UploadPhotoAsync(request.Photo, photoId.ToString());
        if (string.IsNullOrEmpty(photoUrl))
            return Result.Failure(DomainErrors.Venue.PhotoUploadFailed);

        var addPhotoResult = venue.AddPhoto(photoId, photoUrl);
        if (addPhotoResult.IsFailure)
            return addPhotoResult;

        return Result.Success();
    }
}
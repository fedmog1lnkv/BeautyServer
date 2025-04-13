using Application.Messaging.Command;
using Domain.Enums;
using Domain.Errors;
using Domain.Repositories.Staffs;
using Domain.Repositories.Venues;
using Domain.Shared;

namespace Application.Features.Venues.Commands.RemoveVenuePhoto;

public class RemoveVenuePhotoCommandHandler(
    IVenueRepository venueRepository,
    IStaffReadOnlyRepository staffReadOnlyRepository) :
    ICommandHandler<RemoveVenuePhotoCommand, Result>
{
    public async Task<Result> Handle(RemoveVenuePhotoCommand request, CancellationToken cancellationToken)
    {
        var staff = await staffReadOnlyRepository.GetByIdAsync(request.StaffId, cancellationToken);
        if (staff is null)
            return Result.Failure(DomainErrors.Staff.NotFound(request.StaffId));
        
        var venue = await venueRepository.GetByIdWithPhotos(request.VenueId, cancellationToken);
        if (venue is null)
            return Result.Failure(DomainErrors.Venue.NotFound(request.VenueId));
        
        if (staff.Role != StaffRole.Manager || staff.OrganizationId != venue.OrganizationId)
            return Result.Failure(DomainErrors.Staff.CannotUpdate);

        var photo = venue.Photos.FirstOrDefault(p => p.Id == request.PhotoId);
        if (photo is null)
            return Result.Failure(DomainErrors.Venue.PhotoNotFound);

        var removePhotoResult = venue.RemovePhoto(request.PhotoId);
        if (removePhotoResult.IsFailure)
            return removePhotoResult;
        
        await venueRepository.RemovePhoto(photo.PhotoUrl);

        return Result.Success();
    }
}
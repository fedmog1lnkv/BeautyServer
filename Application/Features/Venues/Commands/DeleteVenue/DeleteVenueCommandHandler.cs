using Application.Messaging.Command;
using Domain.Errors;
using Domain.Repositories.Venues;
using Domain.Shared;

namespace Application.Features.Venues.Commands.DeleteVenue;

public sealed class DeleteVenueCommandHandler(IVenueRepository venueRepository)
    : ICommandHandler<DeleteVenueCommand, Result>
{
    public async Task<Result> Handle(DeleteVenueCommand request, CancellationToken cancellationToken)
    {
        var venue = await venueRepository.GetByIdAsync(request.Id, cancellationToken);

        if (venue is null)
            return Result.Failure(DomainErrors.Venue.NotFound(request.Id));

        venueRepository.Remove(venue);

        return Result.Success();
    }
}
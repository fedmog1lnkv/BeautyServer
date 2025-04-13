using Application.Messaging.Command;
using Domain.Shared;

namespace Application.Features.Venues.Commands.RemoveVenuePhoto;

public record RemoveVenuePhotoCommand(Guid StaffId, Guid VenueId, Guid PhotoId) : ICommand<Result>;
using Application.Messaging.Command;
using Domain.Shared;

namespace Application.Features.Venues.Commands.AddVenuePhoto;

public record AddVenuePhotoCommand(Guid StaffId, Guid VenueId, string Photo) : ICommand<Result>;
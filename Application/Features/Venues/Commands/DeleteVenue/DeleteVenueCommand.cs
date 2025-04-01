using Application.Messaging.Command;
using Domain.Shared;

namespace Application.Features.Venues.Commands.DeleteVenue;

public record DeleteVenueCommand(Guid Id) : ICommand<Result>;
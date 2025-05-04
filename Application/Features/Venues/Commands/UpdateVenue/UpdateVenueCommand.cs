using Application.Messaging.Command;
using Domain.Shared;

namespace Application.Features.Venues.Commands.UpdateVenue;

public record UpdateVenueCommand(
    Guid InitiatorId,
    Guid Id,
    string? Name,
    string? Description,
    string? Address,
    string? Color,
    string? Photo,
    double? Latitude,
    double? Longitude,
    List<Guid>? ServiceIds,
    List<Guid>? Photos // for ordering
) : ICommand<Result>;
using Application.Messaging.Command;
using Domain.Shared;

namespace Application.Features.Venues.Commands.CreateVenue;

public record CreateVenueCommand(
    Guid OrganizationId,
    string Name,
    string? Description,
    string? Color,
    string? Photo,
    double Latitude,
    double Longitude) : ICommand<Result>;
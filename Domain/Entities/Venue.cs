using Domain.Errors;
using Domain.Primitives;
using Domain.Repositories.Organizations;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entities;

public class Venue : AggregateRoot
{
    private readonly List<Service> _services = [];

    private Venue(
        Guid id,
        Guid organizationId,
        VenueName name,
        VenueTheme theme,
        Location location,
        VenueDescription? description) : base(id)
    {
        OrganizationId = organizationId;
        Name = name;
        Theme = theme;
        Location = location;
        Description = description;
    }

#pragma warning disable CS8618
    private Venue() { }
#pragma warning restore CS8618

    public Guid OrganizationId { get; private set; }
    public VenueName Name { get; private set; }
    public VenueDescription? Description { get; private set; }
    public VenueTheme Theme { get; private set; }
    public Location Location { get; private set; }
    public IReadOnlyCollection<Service> Services => _services.AsReadOnly();

    public static async Task<Result<Venue>> CreateAsync(
        Guid id,
        Guid organizationId,
        string name,
        string defaultColor,
        double latitude,
        double longitude,
        IOrganizationReadOnlyRepository repository)
    {
        if (organizationId == Guid.Empty)
            return Result.Failure<Venue>(DomainErrors.Venue.OrganizationIdEmpty);

        var organizationExists = await repository.ExistsAsync(organizationId);
        if (!organizationExists)
            return Result.Failure<Venue>(DomainErrors.Organization.NotFound(organizationId));

        var nameResult = VenueName.Create(name);
        if (nameResult.IsFailure)
            return Result.Failure<Venue>(nameResult.Error);

        var themeResult = VenueTheme.Create(defaultColor, null);
        if (themeResult.IsFailure)
            return Result.Failure<Venue>(themeResult.Error);

        var locationResult = Location.Create(latitude, longitude);
        if (locationResult.IsFailure)
            return Result.Failure<Venue>(locationResult.Error);

        return new Venue(
            id,
            organizationId,
            nameResult.Value,
            themeResult.Value,
            locationResult.Value,
            null);
    }

    public Result SetName(string name)
    {
        var nameResult = VenueName.Create(name);
        if (nameResult.IsFailure)
            return nameResult;

        if (Name.Equals(nameResult.Value))
            return Result.Success();

        Name = nameResult.Value;
        return Result.Success();
    }

    public Result SetDescription(string description)
    {
        var descriptionResult = VenueDescription.Create(description);
        if (descriptionResult.IsFailure)
            return descriptionResult;

        if (Description is not null && Description.Equals(descriptionResult.Value))
            return Result.Success();

        Description = descriptionResult.Value;
        return Result.Success();
    }

    public Result SetLocation(double latitude, double longitude)
    {
        var locationResult = Location.Create(latitude, longitude);
        if (locationResult.IsFailure)
            return Result.Failure<Venue>(locationResult.Error);

        if (Location.Equals(locationResult.Value))
            return Result.Success();

        Location = locationResult.Value;
        return Result.Success();
    }

    public Result SetColor(string color)
    {
        return SetTheme(color, Theme.Photo);
    }

    public Result SetPhoto(string? photo)
    {
        return SetTheme(Theme.Color, photo);
    }

    private Result SetTheme(string color, string? photo)
    {
        var themeResult = VenueTheme.Create(color, photo);
        if (themeResult.IsFailure)
            return themeResult;

        if (Theme.Equals(themeResult.Value))
            return Result.Success();

        Theme = themeResult.Value;
        return Result.Success();
    }

    public Result SetServices(IEnumerable<Service> services)
    {
        var validServices = services.Where(s => s.OrganizationId == OrganizationId)
            .DistinctBy(s => s.Id)
            .ToList();

        _services.Clear();
        _services.AddRange(validServices);

        return Result.Success();
    }
}
using Domain.Errors;
using Domain.Primitives;
using Domain.Repositories.Organizations;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entities;

public class Venue : AggregateRoot
{
    private Venue(
        Guid id,
        Guid organizationId,
        VenueName name,
        VenueTheme theme,
        VenueDescription? description) : base(id)
    {
        OrganizationId = organizationId;
        Name = name;
        Theme = theme;
        Description = description;
    }

#pragma warning disable CS8618
    private Venue() { }
#pragma warning restore CS8618

    public Guid OrganizationId { get; private set; }
    public VenueName Name { get; private set; }
    public VenueDescription? Description { get; private set; }
    public VenueTheme Theme { get; private set; }

    public static async Task<Result<Venue>> Create(
        Guid id,
        Guid organizationId,
        string name,
        string defaultColor,
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

        return new Venue(
            id,
            organizationId,
            nameResult.Value,
            themeResult.Value,
            null);
    }

    public Result UpdateName(string name)
    {
        var nameResult = VenueName.Create(name);
        if (nameResult.IsFailure)
            return nameResult;

        if (Name.Equals(nameResult.Value))
            return Result.Success();

        Name = nameResult.Value;
        return Result.Success();
    }

    public Result UpdateDescription(string description)
    {
        var descriptionResult = VenueDescription.Create(description);
        if (descriptionResult.IsFailure)
            return descriptionResult;

        if (Description is not null && Description.Equals(descriptionResult.Value))
            return Result.Success();

        Description = descriptionResult.Value;
        return Result.Success();
    }

    public Result UpdateColor(string color)
    {
        return UpdateTheme(color, Theme.Photo);
    }

    public Result UpdatePhoto(string? photo)
    {
        return UpdateTheme(Theme.Color, photo);
    }

    private Result UpdateTheme(string color, string? photo)
    {
        var themeResult = VenueTheme.Create(color, photo);
        if (themeResult.IsFailure)
            return themeResult;

        if (Theme.Equals(themeResult.Value))
            return Result.Success();

        Theme = themeResult.Value;
        return Result.Success();
    }
}
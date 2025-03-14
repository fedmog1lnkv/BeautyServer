using Domain.Enums;
using Domain.Primitives;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entities;

public sealed class Organization : AggregateRoot
{
    private readonly List<Venue> _venues = [];

    private Organization(
        Guid id,
        OrganizationName name,
        OrganizationTheme theme,
        OrganizationSubscription subscription,
        OrganizationDescription? description) : base(id)
    {
        Name = name;
        Theme = theme;
        Subscription = subscription;
        Description = description;
    }

#pragma warning disable CS8618
    private Organization() { }
#pragma warning restore CS8618

    public OrganizationName Name { get; private set; }
    public OrganizationDescription? Description { get; private set; }
    public OrganizationSubscription Subscription { get; private set; }
    public OrganizationTheme Theme { get; private set; }
    public IReadOnlyCollection<Venue> Venues => _venues.AsReadOnly();


    public static Result<Organization> Create(
        Guid id,
        string name,
        string defaultColor)
    {
        var nameResult = OrganizationName.Create(name);
        if (nameResult.IsFailure)
            return Result.Failure<Organization>(nameResult.Error);

        var themeResult = OrganizationTheme.Create(defaultColor, null);
        if (themeResult.IsFailure)
            return Result.Failure<Organization>(themeResult.Error);

        return new Organization(
            id,
            nameResult.Value,
            themeResult.Value,
            OrganizationSubscription.Disabled,
            null);
    }

    public Result SetName(string name)
    {
        var nameResult = OrganizationName.Create(name);
        if (nameResult.IsFailure)
            return nameResult;

        if (Name.Equals(nameResult.Value))
            return Result.Success();

        Name = nameResult.Value;
        return Result.Success();
    }

    public Result SetDescription(string description)
    {
        var descriptionResult = OrganizationDescription.Create(description);
        if (descriptionResult.IsFailure)
            return descriptionResult;

        if (Description is not null && Description.Equals(descriptionResult.Value))
            return Result.Success();

        Description = descriptionResult.Value;
        return Result.Success();
    }

    public Result SetSubscription(OrganizationSubscription subscription)
    {
        Subscription = subscription;
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
        var themeResult = OrganizationTheme.Create(color, photo);
        if (themeResult.IsFailure)
            return themeResult;

        if (Theme.Equals(themeResult.Value))
            return Result.Success();

        Theme = themeResult.Value;
        return Result.Success();
    }
}
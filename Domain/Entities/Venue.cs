using Domain.Errors;
using Domain.Primitives;
using Domain.Repositories.Organizations;
using Domain.Repositories.Utils;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entities;

public class Venue : AggregateRoot, IAuditableEntity
{
    private readonly List<Service> _services = [];
    private readonly List<VenuePhoto> _photos = [];

    private Venue(
        Guid id,
        Guid organizationId,
        VenueName name,
        VenueTheme theme,
        Location location,
        TimeZoneInfo tz,
        VenueRating rating,
        VenueAddress address,
        VenueDescription? description,
        DateTime createdOnUtc) : base(id)
    {
        OrganizationId = organizationId;
        Name = name;
        Theme = theme;
        Location = location;
        TimeZone = tz;
        Rating = rating;
        Address = address;
        Description = description;

        CreatedOnUtc = createdOnUtc;
    }

#pragma warning disable CS8618
    private Venue() { }
#pragma warning restore CS8618

    public Guid OrganizationId { get; private set; }
    public VenueName Name { get; private set; }
    public VenueDescription? Description { get; private set; }
    public VenueAddress Address { get; private set; }
    public VenueTheme Theme { get; private set; }
    public Location Location { get; private set; }
    public TimeZoneInfo TimeZone { get; private set; }
    public VenueRating Rating { get; private set; }

    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }
    public IReadOnlyCollection<Service> Services => _services.AsReadOnly();
    public IReadOnlyCollection<VenuePhoto> Photos => _photos.AsReadOnly();

    public static async Task<Result<Venue>> CreateAsync(
        Guid id,
        Guid organizationId,
        string name,
        string address,
        string defaultColor,
        double latitude,
        double longitude,
        DateTime createdOnUtc,
        IOrganizationReadOnlyRepository repository,
        ILocationRepository locationRepository)
    {
        if (organizationId == Guid.Empty)
            return Result.Failure<Venue>(DomainErrors.Venue.OrganizationIdEmpty);

        var organizationExists = await repository.ExistsAsync(organizationId);
        if (!organizationExists)
            return Result.Failure<Venue>(DomainErrors.Organization.NotFound(organizationId));

        var nameResult = VenueName.Create(name);
        if (nameResult.IsFailure)
            return Result.Failure<Venue>(nameResult.Error);

        var addressResult = VenueAddress.Create(address);
        if (addressResult.IsFailure)
            return Result.Failure<Venue>(addressResult.Error);

        var themeResult = VenueTheme.Create(defaultColor, null);
        if (themeResult.IsFailure)
            return Result.Failure<Venue>(themeResult.Error);

        var locationResult = Location.Create(latitude, longitude);
        if (locationResult.IsFailure)
            return Result.Failure<Venue>(locationResult.Error);

        var tz = locationRepository.GetTimeZoneByLocation(
            locationResult.Value.Latitude,
            locationResult.Value.Longitude);

        var createRatingResult = VenueRating.Create(0);
        if (createRatingResult.IsFailure)
            return Result.Failure<Venue>(createRatingResult.Error);

        return new Venue(
            id,
            organizationId,
            nameResult.Value,
            themeResult.Value,
            locationResult.Value,
            tz,
            createRatingResult.Value,
            addressResult.Value,
            null,
            createdOnUtc);
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

    public Result SetAddress(string address)
    {
        var addressResult = VenueAddress.Create(address);
        if (addressResult.IsFailure)
            return addressResult;

        if (Address.Equals(addressResult.Value))
            return Result.Success();

        Address = addressResult.Value;
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
    
    public Result SetRating(double rating)
    {
        var ratingResult = VenueRating.Create(rating);
        if (ratingResult.IsFailure)
            return ratingResult;

        if (Rating.Equals(ratingResult.Value))
            return Result.Success();

        Rating = ratingResult.Value;
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

    public Result AddPhoto(Guid id, string photoUrl)
    {
        if (_photos.Any(p => p.PhotoUrl == photoUrl))
            return Result.Failure(DomainErrors.Venue.PhotoAlreadyExists);

        var createPhotoResult = VenuePhoto.Create(
            id,
            Id,
            _photos.Count + 1,
            photoUrl);
        if (createPhotoResult.IsFailure)
            return createPhotoResult;

        _photos.Add(createPhotoResult.Value);
        return Result.Success();
    }

    public Result RemovePhoto(Guid photoId)
    {
        var photo = _photos.FirstOrDefault(p => p.Id == photoId);
        if (photo == null)
            return Result.Failure(DomainErrors.Venue.PhotoNotFound);

        _photos.Remove(photo);
        return ReorderPhotos();
    }

    private Result ReorderPhotos()
    {
        for (int i = 0; i < _photos.Count; i++)
        {
            var photo = _photos[i];
            var setOrderResult = photo.SetOrder(i + 1);
            if (setOrderResult.IsFailure)
                return setOrderResult;
        }

        return Result.Success();
    }

    public Result ReorderPhotos(List<Guid> orderedPhotoIds)
    {
        if (orderedPhotoIds.Count != _photos.Count)
            return Result.Failure(DomainErrors.Venue.InvalidPhotoOrder);

        if (orderedPhotoIds.Distinct().Count() != orderedPhotoIds.Count)
            return Result.Failure(DomainErrors.Venue.DuplicatePhotoIds);

        for (int i = 0; i < orderedPhotoIds.Count; i++)
        {
            var photoId = orderedPhotoIds[i];
            var photo = _photos.FirstOrDefault(p => p.Id == photoId);

            if (photo == null)
                return Result.Failure(DomainErrors.Venue.PhotoNotFound);

            var setOrderResult = photo.SetOrder(i + 1);
            if (setOrderResult.IsFailure)
                return setOrderResult;
        }

        return Result.Success();
    }
}
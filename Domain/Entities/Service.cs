using Domain.Errors;
using Domain.Primitives;
using Domain.Repositories.Organizations;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entities;

public class Service : AggregateRoot, IAuditableEntity
{
    private readonly List<Staff> _staffs = [];
    private readonly List<Venue> _venues = [];

    private Service(
        Guid id,
        Guid organizationId,
        ServiceName name,
        ServiceRating rating,
        ServiceDescription? description,
        TimeSpan? duration,
        ServicePrice? price,
        ServicePhoto? photo,
        DateTime createdOnUtc) : base(id)
    {
        OrganizationId = organizationId;
        Name = name;
        Rating = rating;
        Description = description;
        Duration = duration;
        Price = price;
        Photo = photo;

        CreatedOnUtc = createdOnUtc;
    }

#pragma warning disable CS8618
    private Service() { }
#pragma warning restore CS8618

    public Guid OrganizationId { get; private set; }
    public ServiceName Name { get; private set; }
    public ServiceDescription? Description { get; private set; }
    public ServiceRating Rating { get; private set; }
    public TimeSpan? Duration { get; private set; }
    public ServicePrice? Price { get; private set; }
    public ServicePhoto? Photo { get; private set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    public IReadOnlyCollection<Staff> Staffs => _staffs.AsReadOnly();
    public IReadOnlyCollection<Venue> Venues => _venues.AsReadOnly();

    public static async Task<Result<Service>> Create(
        Guid id,
        Guid organizationId,
        string name,
        DateTime createdOnUtc,
        IOrganizationReadOnlyRepository repository)
    {
        if (organizationId == Guid.Empty)
            return Result.Failure<Service>(DomainErrors.Service.OrganizationIdEmpty);

        var organizationExists = await repository.ExistsAsync(organizationId);
        if (!organizationExists)
            return Result.Failure<Service>(DomainErrors.Organization.NotFound(organizationId));

        var nameResult = ServiceName.Create(name);
        if (nameResult.IsFailure)
            return Result.Failure<Service>(nameResult.Error);

        var createRatingResult = ServiceRating.Create(0);
        if (createRatingResult.IsFailure)
            return Result.Failure<Service>(createRatingResult.Error);

        return new Service(
            id,
            organizationId,
            nameResult.Value,
            createRatingResult.Value,
            null,
            null,
            null,
            null,
            createdOnUtc);
    }

    public Result SetName(string name)
    {
        var nameResult = ServiceName.Create(name);
        if (nameResult.IsFailure)
            return nameResult;

        if (Name.Equals(nameResult.Value))
            return Result.Success();

        Name = nameResult.Value;
        return Result.Success();
    }

    public Result SetRating(double rating)
    {
        var ratingResult = ServiceRating.Create(rating);
        if (ratingResult.IsFailure)
            return ratingResult;

        if (Rating.Equals(ratingResult.Value))
            return Result.Success();

        Rating = ratingResult.Value;
        return Result.Success();
    }

    public Result SetDescription(string description)
    {
        var descriptionResult = ServiceDescription.Create(description);
        if (descriptionResult.IsFailure)
            return descriptionResult;

        if (Description is not null && Description.Equals(descriptionResult.Value))
            return Result.Success();

        Description = descriptionResult.Value;
        return Result.Success();
    }

    public Result SetDuration(int durationMinutes)
    {
        if (durationMinutes <= 0)
            return Result.Failure(DomainErrors.Service.InvalidDuration);

        var newDuration = TimeSpan.FromMinutes(durationMinutes);
        if (Duration.HasValue && Duration.Value == newDuration)
            return Result.Success();

        Duration = newDuration;
        return Result.Success();
    }

    public Result SetPrice(double price)
    {
        var priceResult = ServicePrice.Create(price);
        if (priceResult.IsFailure)
            return priceResult;

        if (Price is not null && Price.Equals(priceResult.Value))
            return Result.Success();

        Price = priceResult.Value;
        return Result.Success();
    }

    public Result SetPhoto(string photoUrl)
    {
        var photoResult = ServicePhoto.Create(photoUrl);
        if (photoResult.IsFailure)
            return photoResult;

        if (Photo is not null && Photo.Equals(photoResult.Value))
            return Result.Success();

        Photo = photoResult.Value;
        return Result.Success();
    }
    
    public Result SetVenue(IEnumerable<Venue> venues)
    {
        var validVenues = venues.Where(v => v.OrganizationId == OrganizationId)
            .DistinctBy(v => v.Id)
            .ToList();

        _venues.Clear();
        _venues.AddRange(validVenues);

        return Result.Success();
    }
    
    public Result SetStaff(IEnumerable<Staff> staffs)
    {
        var validStaffs = staffs.Where(s => s.OrganizationId == OrganizationId)
            .DistinctBy(s => s.Id)
            .ToList();

        _staffs.Clear();
        _staffs.AddRange(validStaffs);

        return Result.Success();
    }
}
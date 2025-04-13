using Domain.Errors;
using Domain.Primitives;
using Domain.Shared;

namespace Domain.Entities;

public class VenuePhoto : Entity
{
    private VenuePhoto(
        Guid id,
        Guid venueId,
        int order,
        string photoUrl) : base(id)
    {
        VenueId = venueId;
        Order = order;
        PhotoUrl = photoUrl;
    }

#pragma warning disable CS8618
    private VenuePhoto() { }
#pragma warning restore CS8618

    public Guid VenueId { get; private set; }
    public int Order { get; private set; }
    public string PhotoUrl { get; private set; }

    public static Result<VenuePhoto> Create(
        Guid id,
        Guid venueId,
        int order,
        string photoUrl)
    {
        if (string.IsNullOrEmpty(photoUrl))
            return Result.Failure<VenuePhoto>(DomainErrors.VenuePhoto.Empty);

        return new VenuePhoto(
            id,
            venueId,
            order,
            photoUrl);
    }

    public Result SetOrder(int newOrder)
    {
        if (newOrder <= 0)
            return Result.Failure(DomainErrors.VenuePhoto.InvalidOrder);

        if (Order == newOrder)
            return Result.Success();

        Order = newOrder;
        return Result.Success();
    }
}
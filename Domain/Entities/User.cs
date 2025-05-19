using Domain.Errors;
using Domain.Primitives;
using Domain.Repositories.Users;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entities;

public sealed class User : AggregateRoot, IAuditableEntity
{
    private readonly List<Coupon> _coupons = [];
    private User(
        Guid id,
        UserName name,
        UserPhoneNumber phoneNumber,
        UserPhoto? photo,
        UserSettings settings,
        DateTime createdOnUtc) : base(id)
    {
        Name = name;
        PhoneNumber = phoneNumber;
        Photo = photo;
        Settings = settings;

        CreatedOnUtc = createdOnUtc;
    }

#pragma warning disable CS8618
    private User() { }
#pragma warning restore

    public UserName Name { get; private set; }
    public UserPhoneNumber PhoneNumber { get; private set; }
    public UserPhoto? Photo { get; private set; }
    public UserSettings Settings { get; private set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }
    
    public IReadOnlyCollection<Coupon> Coupons => _coupons.AsReadOnly();

    public static async Task<Result<User>> CreateAsync(
        Guid id,
        string name,
        string phoneNumber,
        DateTime createdOnUtc,
        IUserReadOnlyRepository repository,
        CancellationToken cancellationToken)
    {
        var createNameResult = UserName.Create(name);
        if (createNameResult.IsFailure)
            return Result.Failure<User>(createNameResult.Error);

        var createPhoneNumberResult = UserPhoneNumber.Create(phoneNumber);
        if (createPhoneNumberResult.IsFailure)
            return Result.Failure<User>(createPhoneNumberResult.Error);

        var isUniquePhone = await repository.IsPhoneNumberUnique(createPhoneNumberResult.Value, cancellationToken);
        if (!isUniquePhone)
            return Result.Failure<User>(DomainErrors.User.PhoneNumberNotUnique);

        var createSettingsResult = UserSettings.Create(null, true, true);
        if (createSettingsResult.IsFailure)
            return Result.Failure<User>(createSettingsResult.Error);

        var user = new User(
            id,
            createNameResult.Value,
            createPhoneNumberResult.Value,
            null,
            createSettingsResult.Value,
            createdOnUtc);

        return Result.Success(user);
    }

    public Result SetName(string name)
    {
        var nameResult = UserName.Create(name);
        if (nameResult.IsFailure)
            return nameResult;

        if (Name.Equals(nameResult.Value))
            return Result.Success();

        Name = nameResult.Value;
        return Result.Success();
    }

    public Result SetSettings(
        string? firebaseToken,
        bool receiveOrderNotifications,
        bool receivePromoNotifications)
    {
        var settingsResult = UserSettings.Create(firebaseToken, receiveOrderNotifications, receivePromoNotifications);

        if (settingsResult.IsFailure)
            return settingsResult;

        if (Settings.Equals(settingsResult.Value))
            return Result.Success();

        Settings = settingsResult.Value;

        return Result.Success();
    }

    public Result SetFirebaseToken(string? firebaseToken)
    {
        var settingsResult = UserSettings.Create(
            firebaseToken,
            Settings.ReceiveOrderNotifications,
            Settings.ReceivePromoNotifications);

        if (settingsResult.IsFailure)
            return settingsResult;

        Settings = settingsResult.Value;

        return Result.Success();
    }

    public Result SetNotificationPreferences(bool? receiveOrderNotifications, bool? receivePromoNotifications)
    {
        var newReceiveOrderNotifications = receiveOrderNotifications ?? Settings.ReceiveOrderNotifications;
        var newReceivePromoNotifications = receivePromoNotifications ?? Settings.ReceivePromoNotifications;

        var settingsResult = UserSettings.Create(
            Settings.FirebaseToken,
            newReceiveOrderNotifications,
            newReceivePromoNotifications);

        if (settingsResult.IsFailure)
            return settingsResult;

        Settings = settingsResult.Value;

        return Result.Success();
    }

    public Result SetPhoto(string photoUrl)
    {
        var photoResult = UserPhoto.Create(photoUrl);
        if (photoResult.IsFailure)
            return photoResult;

        if (Photo is not null && Photo.Equals(photoResult.Value))
            return Result.Success();

        Photo = photoResult.Value;
        return Result.Success();
    }
    
    public Result AddCoupon(Coupon coupon)
    {
        if (_coupons.Any(c => c.Id == coupon.Id))
            return Result.Failure(DomainErrors.User.CouponDuplicate);

        _coupons.Add(coupon);

        return Result.Success();
    }
}
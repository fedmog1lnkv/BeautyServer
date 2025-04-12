using Domain.Errors;
using Domain.Primitives;
using Domain.Shared;

namespace Domain.ValueObjects;

public sealed class UserSettings : ValueObject
{
    private UserSettings(
        string? firebaseToken,
        bool receiveOrderNotifications,
        bool receivePromoNotifications)
    {
        FirebaseToken = firebaseToken;
        ReceiveOrderNotifications = receiveOrderNotifications;
        ReceivePromoNotifications = receivePromoNotifications;
    }

    public string? FirebaseToken { get; }
    public bool ReceiveOrderNotifications { get; }
    public bool ReceivePromoNotifications { get; }

    public static Result<UserSettings> Create(
        string? firebaseToken,
        bool receiveOrderNotifications,
        bool receivePromoNotifications)
    {
        if (firebaseToken != null && string.IsNullOrWhiteSpace(firebaseToken))
            return Result.Failure<UserSettings>(DomainErrors.UserSettings.FirebaseTokenEmpty);


        return new UserSettings(firebaseToken, receiveOrderNotifications, receivePromoNotifications);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return FirebaseToken ?? string.Empty;
        yield return ReceiveOrderNotifications;
        yield return ReceivePromoNotifications;
    }
}
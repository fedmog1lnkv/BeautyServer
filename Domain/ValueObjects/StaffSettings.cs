using Domain.Errors;
using Domain.Primitives;
using Domain.Shared;

namespace Domain.ValueObjects;

public sealed class StaffSettings : ValueObject
{
    private StaffSettings(string? firebaseToken) => FirebaseToken = firebaseToken;

    public string? FirebaseToken { get; }

    public static Result<StaffSettings> Create(string? firebaseToken)
    {
        if (firebaseToken != null && string.IsNullOrWhiteSpace(firebaseToken))
            return Result.Failure<StaffSettings>(DomainErrors.StaffSettings.FirebaseTokenEmpty);


        return new StaffSettings(firebaseToken);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return FirebaseToken ?? string.Empty;
    }
}
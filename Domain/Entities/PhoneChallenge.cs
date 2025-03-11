using Domain.Primitives;
using Domain.Shared;

namespace Domain.Entities;

public class PhoneChallenge : AggregateRoot
{
    public Guid Id { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string PhoneNumber { get; private set; }
    public string VerificationCode { get; private set; }
    public DateTime ExpiredAt { get; private set; }

    private PhoneChallenge(
        Guid id,
        string phoneNumber,
        string verificationCode,
        DateTime createdAt,
        DateTime expiredAt)
    {
        Id = id;
        CreatedAt = createdAt;
        PhoneNumber = phoneNumber;
        VerificationCode = verificationCode;
        ExpiredAt = expiredAt;
    }

    public static Result<PhoneChallenge> Create(
        Guid id,
        string phoneNumber,
        string verificationCode,
        TimeSpan duration)
    {
        var createdAt = DateTime.UtcNow;
        var expiredAt = createdAt.Add(duration);
        return new PhoneChallenge(id, phoneNumber, verificationCode, createdAt, expiredAt);
    }

    public bool IsExpired() => DateTime.UtcNow > ExpiredAt;
}
using Domain.Entities;
using Domain.Repositories.Base;

namespace Domain.Repositories.PhoneChallenges;

public interface IPhoneChallengeRepository : IRepository<PhoneChallenge>
{
    Task<PhoneChallenge?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default);
    Task<string?> SendAuthRequestAsync(string phoneNumber, CancellationToken cancellationToken = default);

    Task<bool> SendCodeAsync(
        string phoneNumber,
        string code,
        CancellationToken cancellationToken = default);

    void Add(PhoneChallenge phoneChallenge);
    void Remove(PhoneChallenge phoneChallenge);
}
using Application.Messaging.Command;
using Domain.Entities;
using Domain.Errors;
using Domain.Repositories.PhoneChallenges;
using Domain.Repositories.Users;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Features.User.Commands.GeneratePhoneChallenge;

public class GeneratePhoneChallengeCommandHandler(
    IUserRepository userRepository,
    IUserReadOnlyRepository userReadOnlyRepository,
    IPhoneChallengeRepository phoneChallengeRepository)
    : ICommandHandler<GeneratePhoneChallengeCommand, Result>
{
    public async Task<Result> Handle(GeneratePhoneChallengeCommand request, CancellationToken cancellationToken)
    {
        var userPhoneNumberResult = UserPhoneNumber.Create(request.PhoneNumber);
        if (userPhoneNumberResult.IsFailure)
            return Result.Failure(userPhoneNumberResult.Error);

        var userPhoneNumber = userPhoneNumberResult.Value;

        var user = await userRepository.GetByPhoneNumberAsync(userPhoneNumber, cancellationToken);
        if (user is null)
        {
            var userName = await phoneChallengeRepository.SendAuthRequestAsync(userPhoneNumber.Value, cancellationToken);
            if (userName == null)
                return Result.Failure(DomainErrors.User.RejectAuthRequest);

            var createUserResult = await Domain.Entities.User.CreateAsync(
                Guid.NewGuid(),
                userName,
                userPhoneNumber.Value,
                userReadOnlyRepository,
                cancellationToken);
            if (createUserResult.IsFailure)
                return Result.Failure(createUserResult.Error);

            userRepository.Add(createUserResult.Value);

            user = createUserResult.Value;
        }

        var oldPhoneChallenge = await phoneChallengeRepository.GetByPhoneNumberAsync(userPhoneNumber.Value, cancellationToken);
        if (oldPhoneChallenge is not null)
            phoneChallengeRepository.Remove(oldPhoneChallenge);

        var code = new Random().Next(100000, 999999).ToString(); // Generate 6 digit code
        var createPhoneChallengeResult = PhoneChallenge.Create(
            Guid.NewGuid(),
            userPhoneNumber.Value,
            code,
            TimeSpan.FromMinutes(5));

        phoneChallengeRepository.Add(createPhoneChallengeResult.Value);

        var sendCodeResult = await phoneChallengeRepository.SendCodeAsync(user.PhoneNumber.Value, code);
        return !sendCodeResult
            ? Result.Failure(DomainErrors.PhoneChallenge.NotSend)
            : Result.Success();
    }
}
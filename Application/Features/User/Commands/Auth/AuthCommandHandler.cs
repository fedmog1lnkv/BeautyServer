using Application.Abstractions;
using Application.Features.User.Commands.Auth.Dto;
using Application.Messaging.Command;
using Domain.Errors;
using Domain.Repositories.PhoneChallenges;
using Domain.Repositories.Users;
using Domain.Shared;
using Domain.ValueObjects;
using Microsoft.Extensions.Configuration;

namespace Application.Features.User.Commands.Auth;

public class AuthCommandHandler(
    IJwtProvider jwtProvider,
    IUserRepository userRepository,
    IPhoneChallengeRepository phoneChallengeRepository,
    IConfiguration configuration)
    : ICommandHandler<AuthCommand, Result<AuthVm>>
{
    public async Task<Result<AuthVm>> Handle(
        AuthCommand request,
        CancellationToken cancellationToken)
    {
        var userPhoneNumberResult = UserPhoneNumber.Create(request.PhoneNumber);
        if (userPhoneNumberResult.IsFailure)
            return Result.Failure<AuthVm>(userPhoneNumberResult.Error);

        var userPhoneNumber = userPhoneNumberResult.Value;

        var phoneChallenge = await phoneChallengeRepository.GetByPhoneNumberAsync(userPhoneNumber.Value);
        if (phoneChallenge == null)
            return Result.Failure<AuthVm>(DomainErrors.PhoneChallenge.NotFound);

        if (phoneChallenge.ExpiredAt < DateTime.UtcNow)
        {
            return Result.Failure<AuthVm>(DomainErrors.PhoneChallenge.Expired);
        }

        if (request.Code != phoneChallenge.VerificationCode)
        {
            return Result.Failure<AuthVm>(DomainErrors.PhoneChallenge.InvalidCode);
        }

        var user = await userRepository.GetByPhoneNumberAsync(userPhoneNumber, cancellationToken);
        if (user == null)
        {
            return Result.Failure<AuthVm>(DomainErrors.User.NotFoundByPhone(userPhoneNumber));
        }

        // Читаем список администраторов из конфигурации
        var adminPhones = configuration["AdminPhoneNumbers"]?.Split(',') ?? Array.Empty<string>();

        // Проверка, является ли пользователь администратором
        var isAdmin = adminPhones.Any(adminPhone =>
        {
            try
            {
                var normalizedAdminPhone = UserPhoneNumber.Create(adminPhone);
                return userPhoneNumber.Equals(normalizedAdminPhone);
            }
            catch
            {
                return false;
            }
        });

        var accessToken = jwtProvider.GenerateToken(user.Id, isAdmin);
        var refreshToken = jwtProvider.GenerateRefreshToken(user.Id, isAdmin);

        phoneChallengeRepository.Remove(phoneChallenge);

        var authVm = new AuthVm
        {
            Token = accessToken,
            RefreshToken = refreshToken
        };

        return Result.Success(authVm);
    }
}
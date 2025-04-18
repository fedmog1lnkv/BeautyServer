using Application.Abstractions;
using Application.Features.Staffs.Commands.Auth.Dto;
using Application.Messaging.Command;
using Domain.Enums;
using Domain.Errors;
using Domain.Repositories.PhoneChallenges;
using Domain.Repositories.Staffs;
using Domain.Shared;
using Domain.ValueObjects;
using Microsoft.Extensions.Configuration;

namespace Application.Features.Staffs.Commands.Auth;

public class AuthStaffCommandHandler(
    IStaffJwtProvider jwtProvider,
    IStaffRepository staffRepository,
    IPhoneChallengeRepository phoneChallengeRepository,
    IConfiguration configuration)
    : ICommandHandler<AuthStaffCommand, Result<AuthVm>>
{
    public async Task<Result<AuthVm>> Handle(AuthStaffCommand request, CancellationToken cancellationToken)
    {
        var staffPhoneNumberResult = StaffPhoneNumber.Create(request.PhoneNumber);
        if (staffPhoneNumberResult.IsFailure)
            return Result.Failure<AuthVm>(staffPhoneNumberResult.Error);

        var staffPhoneNumber = staffPhoneNumberResult.Value;

        var phoneChallenge =
            await phoneChallengeRepository.GetByPhoneNumberAsync(staffPhoneNumber.Value, cancellationToken);
        if (phoneChallenge is null)
            return Result.Failure<AuthVm>(DomainErrors.PhoneChallenge.NotFound);

        if (phoneChallenge.ExpiredAt < DateTime.UtcNow)
            return Result.Failure<AuthVm>(DomainErrors.PhoneChallenge.Expired);

        if (request.Code != phoneChallenge.VerificationCode)
            return Result.Failure<AuthVm>(DomainErrors.PhoneChallenge.InvalidCode);

        var staff = await staffRepository.GetByPhoneNumberAsync(staffPhoneNumber, cancellationToken);
        if (staff == null)
        {
            return Result.Failure<AuthVm>(DomainErrors.Staff.NotFoundByPhone(staffPhoneNumber));
        }

        var accessToken = jwtProvider.GenerateToken(
            staff.Id,
            staff.OrganizationId,
            staff.Role == StaffRole.Manager);

        var refreshToken = jwtProvider.GenerateRefreshToken(
            staff.Id,
            staff.OrganizationId,
            staff.Role == StaffRole.Manager);

        phoneChallengeRepository.Remove(phoneChallenge);

        var authVm = new AuthVm
        {
            Token = accessToken,
            RefreshToken = refreshToken
        };

        return Result.Success(authVm);
    }
}
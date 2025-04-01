using Application.Messaging.Command;
using Domain.Entities;
using Domain.Errors;
using Domain.Repositories.Organizations;
using Domain.Repositories.PhoneChallenges;
using Domain.Repositories.Staffs;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Features.Staffs.Commands.GeneratePhoneChallenge;

public class GenerateStaffPhoneChallengeCommandHandler(
    IOrganizationRepository organizationRepository,
    IOrganizationReadOnlyRepository organizationReadOnlyRepository,
    IStaffRepository staffRepository,
    IStaffReadOnlyRepository staffReadOnlyRepository,
    IPhoneChallengeRepository phoneChallengeRepository)
    : ICommandHandler<GenerateStaffPhoneChallengeCommand, Result>
{
    public async Task<Result> Handle(
        GenerateStaffPhoneChallengeCommand request,
        CancellationToken cancellationToken)
    {
        var isOrganizationExistsAsync = await organizationReadOnlyRepository.ExistsAsync(
            request.OrganizationId,
            cancellationToken);

        if (!isOrganizationExistsAsync)
            return Result.Failure(DomainErrors.Organization.NotFound(request.OrganizationId));

        var staffPhoneNumberResult = StaffPhoneNumber.Create(request.PhoneNumber);
        if (staffPhoneNumberResult.IsFailure)
            return Result.Failure(staffPhoneNumberResult.Error);

        var staffPhoneNumber = staffPhoneNumberResult.Value;

        var staff = await staffRepository.GetByPhoneNumberAsync(staffPhoneNumber, cancellationToken);
        if (staff is null)
        {
            var staffName = await phoneChallengeRepository.SendAuthRequestAsync(
                staffPhoneNumber.Value,
                cancellationToken);
            if (staffName == null)
                return Result.Failure(DomainErrors.User.RejectAuthRequest);

            var createStaffResult = await Staff.CreateAsync(
                Guid.NewGuid(),
                request.OrganizationId,
                staffName,
                staffPhoneNumber.Value,
                DateTime.UtcNow,
                staffReadOnlyRepository,
                organizationReadOnlyRepository,
                cancellationToken);
            if (createStaffResult.IsFailure)
                return Result.Failure(createStaffResult.Error);

            staffRepository.Add(createStaffResult.Value);

            staff = createStaffResult.Value;
        }

        var oldPhoneChallenge =
            await phoneChallengeRepository.GetByPhoneNumberAsync(staffPhoneNumber.Value, cancellationToken);
        if (oldPhoneChallenge is not null)
            phoneChallengeRepository.Remove(oldPhoneChallenge);

        var code = new Random().Next(100000, 999999).ToString(); // Generate 6 digit code
        var createPhoneChallengeResult = PhoneChallenge.Create(
            Guid.NewGuid(),
            staffPhoneNumber.Value,
            code,
            TimeSpan.FromMinutes(5));

        phoneChallengeRepository.Add(createPhoneChallengeResult.Value);

        var sendCodeResult = await phoneChallengeRepository.SendCodeAsync(staff.PhoneNumber.Value, code);
        return !sendCodeResult
            ? Result.Failure(DomainErrors.PhoneChallenge.NotSend)
            : Result.Success();
    }
}
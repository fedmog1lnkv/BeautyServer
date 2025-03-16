using Application.Messaging.Command;
using Domain.Shared;

namespace Application.Features.Staffs.Commands.GeneratePhoneChallenge;

public record GenerateStaffPhoneChallengeCommand(string PhoneNumber, Guid OrganizationId) : ICommand<Result>;
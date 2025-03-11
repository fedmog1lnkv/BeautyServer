using Application.Messaging.Command;
using Domain.Shared;

namespace Application.Features.User.Commands.GeneratePhoneChallenge;

public record GeneratePhoneChallengeCommand(string PhoneNumber) : ICommand<Result>;
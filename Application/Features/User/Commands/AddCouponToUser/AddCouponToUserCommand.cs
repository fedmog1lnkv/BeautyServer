using Application.Messaging.Command;
using Domain.Shared;

namespace Application.Features.User.Commands.AddCouponToUser;

public record AddCouponToUserCommand(Guid UserId, string CouponCode) : ICommand<Result>;
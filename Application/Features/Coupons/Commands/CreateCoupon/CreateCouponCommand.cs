using Application.Messaging.Command;
using Domain.Shared;

namespace Application.Features.Coupons.Commands.CreateCoupon;

public record CreateCouponCommand(
    Guid InitiatorId,
    Guid OrganizationId,
    string Name,
    string Description,
    string DiscountType,
    decimal DiscountValue,
    bool IsPublic,
    int UsageLimit,
    DateOnly StartDate,
    DateOnly EndDate,
    List<Guid>? ServiceIds) : ICommand<Result>;
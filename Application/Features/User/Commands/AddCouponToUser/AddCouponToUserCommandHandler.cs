using Application.Messaging.Command;
using Domain.Errors;
using Domain.Repositories.Coupons;
using Domain.Repositories.Users;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Features.User.Commands.AddCouponToUser;

public class AddCouponToUserCommandHandler(IUserRepository userRepository, ICouponRepository couponRepository) :
    ICommandHandler<AddCouponToUserCommand, Result>
{
    public async Task<Result> Handle(AddCouponToUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdWithCouponsAsync(request.UserId, cancellationToken);
        if (user == null)
            return Result.Failure(DomainErrors.User.NotFound(request.UserId));

        var createCodeResult = CouponCode.Create(request.CouponCode);
        if (createCodeResult.IsFailure)
            return createCodeResult;
        
        var coupon = await couponRepository.GetByCodeAsync(createCodeResult.Value, cancellationToken);
        if (coupon == null)
            return Result.Failure(DomainErrors.Coupon.NotFoundByCode(request.CouponCode));

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (coupon.StartDate > today || coupon.EndDate < today || coupon.UsageLimit.Remaining == 0)
            return Result.Failure(DomainErrors.Coupon.NotActive);

        var addCouponResult = user.AddCoupon(coupon);
        if (addCouponResult.IsFailure)
            return addCouponResult;

        return Result.Success();
    }
}
using Application.Messaging.Command;
using Domain.Entities;
using Domain.Enums;
using Domain.Errors;
using Domain.Repositories.Coupons;
using Domain.Repositories.Organizations;
using Domain.Repositories.Services;
using Domain.Repositories.Staffs;
using Domain.Shared;

namespace Application.Features.Coupons.Commands.CreateCoupon;

public sealed class CreateCouponCommandHandler(
    IStaffRepository staffRepository,
    IOrganizationReadOnlyRepository organizationRepository,
    IServiceRepository serviceRepository,
    ICouponRepository couponRepository) : ICommandHandler<CreateCouponCommand, Result>
{
    public async Task<Result> Handle(CreateCouponCommand request, CancellationToken cancellationToken)
    {
        var staff = await staffRepository.GetByIdAsync(request.InitiatorId, cancellationToken);
        if (staff is null)
            return Result.Failure(DomainErrors.Staff.NotFound(request.InitiatorId));
        
        if (staff.OrganizationId != request.OrganizationId || staff.Role != StaffRole.Manager)
            return Result.Failure(DomainErrors.Staff.NoPermissionToManageOrganization);

        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        var couponCode = new string(Enumerable.Range(0, 10)
            .Select(_ => chars[random.Next(chars.Length)])
            .ToArray());

        var couponResult = await Coupon.Create(
            Guid.NewGuid(),
            request.OrganizationId,
            request.Name,
            request.Description,
            couponCode,
            Enum.Parse<CouponDiscountType>(request.DiscountType, ignoreCase: true),
            request.DiscountValue,
            request.IsPublic,
            request.UsageLimit,
            request.StartDate,
            request.EndDate,
            DateTime.UtcNow,
            organizationRepository,
            cancellationToken);

        if (couponResult.IsFailure)
            return Result.Failure(couponResult.Error);

        var coupon = couponResult.Value;

        couponRepository.Add(coupon);


        if (request.ServiceIds is not null && request.ServiceIds.Count > 0)
        {
            var services = await serviceRepository.GetByIdsAsync(request.ServiceIds, cancellationToken);

            if (services.Count != request.ServiceIds.Count)
                return Result.Failure(DomainErrors.Service.NotFound(Guid.Empty));

            var addServicesResult = coupon.AddServices(services);
            if (addServicesResult.IsFailure)
                return Result.Failure(addServicesResult.Error);
        }
        return Result.Success();
    }
}

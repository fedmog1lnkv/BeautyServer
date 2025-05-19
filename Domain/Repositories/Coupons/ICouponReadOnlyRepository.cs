using Domain.Entities;
using Domain.Repositories.Base;

namespace Domain.Repositories.Coupons;

public interface ICouponReadOnlyRepository : IReadOnlyRepository<Coupon>
{
    Task<List<Coupon>> GetByOrganizationId(Guid organizationId, CancellationToken cancellationToken = default);
    Task<List<Coupon>> GetByUserId(Guid organizationId, CancellationToken cancellationToken = default);
    Task<List<Coupon>> GetPublicCoupons(CancellationToken cancellationToken);
}
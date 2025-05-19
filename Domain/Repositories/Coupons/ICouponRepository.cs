using Domain.Entities;
using Domain.Repositories.Base;
using Domain.ValueObjects;

namespace Domain.Repositories.Coupons;

public interface ICouponRepository : IRepository<Coupon>
{
    Task<Coupon?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Coupon?> GetByCodeAsync(CouponCode code, CancellationToken cancellationToken = default);
    void Add(Coupon coupon);
    void Remove(Coupon coupon);
}
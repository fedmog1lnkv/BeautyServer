using Domain.Entities;
using Domain.Errors;
using Domain.Repositories.Coupons;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Coupons;

public class CouponRepository(ApplicationDbContext dbContext) : ICouponRepository
{
    public async Task<Coupon?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.Set<Coupon>()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<Coupon?> GetByCodeAsync(CouponCode code, CancellationToken cancellationToken = default) =>
        await dbContext.Set<Coupon>()
            .FirstOrDefaultAsync(c => c.Code == code, cancellationToken);

    public void Add(Coupon coupon) =>
        dbContext.Set<Coupon>().Add(coupon);

    public void Remove(Coupon coupon) =>
        dbContext.Set<Coupon>().Remove(coupon);
}
using Domain.Entities;
using Domain.Repositories.Coupons;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Coupons;

public class CouponReadOnlyRepository(ApplicationDbContext dbContext) : ICouponReadOnlyRepository
{
    public async Task<List<Coupon>> GetByOrganizationId(
        Guid organizationId,
        CancellationToken cancellationToken = default) =>
        await dbContext.Set<Coupon>()
            .Where(v => v.OrganizationId == organizationId)
            .ToListAsync(cancellationToken);

    public async Task<List<Coupon>> GetByUserId(Guid userId, CancellationToken cancellationToken = default) =>
        await dbContext.Set<Coupon>()
            .Where(c => c.Users.Any(u => u.Id == userId))
            .ToListAsync(cancellationToken);

    public async Task<List<Coupon>> GetPublicCoupons(CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        return await dbContext.Set<Coupon>()
            .Where(c => c.IsPublic && c.StartDate <= today && c.EndDate >= today)
            .ToListAsync(cancellationToken);
    }
}
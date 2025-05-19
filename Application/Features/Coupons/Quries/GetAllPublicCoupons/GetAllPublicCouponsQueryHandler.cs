using Application.Features.Coupons.Quries.GetAllPublicCoupons.Dto;
using Application.Features.Dto;
using Application.Messaging.Query;
using Domain.Repositories.Coupons;
using Domain.Shared;

namespace Application.Features.Coupons.Quries.GetAllPublicCoupons;

public sealed class GetAllPublicCouponsQueryHandler(ICouponReadOnlyRepository couponReadOnlyRepository) :
    IQueryHandler<GetAllPublicCouponsQuery,
        Result<PaginatedVm<CouponLookupDto>>>
{
    public async Task<Result<PaginatedVm<CouponLookupDto>>> Handle(
        GetAllPublicCouponsQuery request,
        CancellationToken cancellationToken)
    {
        var coupons = await couponReadOnlyRepository.GetPublicCoupons(cancellationToken);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchLower = request.Search.Trim().ToLower();
            coupons = coupons
                .Where(c => c.Name.Value.ToLower().Contains(searchLower))
                .ToList();
        }

        var couponDtos = coupons.Select(c => new CouponLookupDto
        {
            Id = c.Id,
            OrganizationId = c.OrganizationId,
            Name = c.Name.Value,
            Description = c.Description.Value,
            Code = c.Code.Value,
            DiscountType = c.DiscountType.ToString(),
            DiscountValue = c.DiscountValue.Value,
            IsPublic = c.IsPublic,
            UsageLimit = c.UsageLimit.Total,
            StartDate = c.StartDate,
            EndDate = c.EndDate,
        }).ToList();

        var pagedData = couponDtos
            .Skip(request.Page * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var result = new PaginatedVm<CouponLookupDto>
        {
            Data = pagedData,
            TotalCount = couponDtos.Count,
            PageNumber = request.Page,
            PageSize = request.PageSize
        };

        return Result.Success(result);
    }
}
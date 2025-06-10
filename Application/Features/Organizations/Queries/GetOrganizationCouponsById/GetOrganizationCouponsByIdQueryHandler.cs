using Application.Features.Dto;
using Application.Features.Organizations.Queries.GetOrganizationCouponsById.Dto;
using Application.Features.Organizations.Queries.GetOrganizationStaffsById;
using Application.Features.Organizations.Queries.GetOrganizationStaffsById.Dto;
using Application.Messaging.Query;
using Domain.Repositories.Coupons;
using Domain.Repositories.Staffs;
using Domain.Shared;

namespace Application.Features.Organizations.Queries.GetOrganizationCouponsById;

public class GetOrganizationCouponsByIdQueryHandler(ICouponReadOnlyRepository couponReadOnlyRepository)
    : IQueryHandler<GetOrganizationCouponsByIdQuery,
        Result<PaginatedVm<CouponLookupDto>>>
{
    public async Task<Result<PaginatedVm<CouponLookupDto>>> Handle(
        GetOrganizationCouponsByIdQuery request,
        CancellationToken cancellationToken)
    {
        var coupons = await couponReadOnlyRepository.GetByOrganizationId(request.Id, cancellationToken);

        if (request.Search is not null)
        {
            coupons = coupons.Where(s => s.Name.Value.ToLower().Contains(request.Search.ToLower())).ToList();
        }

        var couponDtos = coupons.Select(s => new CouponLookupDto
        {
            Id = s.Id,
            OrganizationId = s.OrganizationId,
            Name = s.Name.Value,
            Description = s.Description.Value,
            Code = s.Code.Value,
            DiscountType = s.DiscountType.ToString(),
            DiscountValue = s.DiscountValue.Value,
            IsPublic = s.IsPublic,
            UsageLimit = s.UsageLimit.Total,
            UsageLimitRemaining = s.UsageLimit.Remaining,
            StartDate = s.StartDate,
            EndDate = s.EndDate,
        }).ToList();

        // TODO : move to GetByOrganizationId
        var pagedData = couponDtos
            .Skip((request.Page) * request.PageSize)
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
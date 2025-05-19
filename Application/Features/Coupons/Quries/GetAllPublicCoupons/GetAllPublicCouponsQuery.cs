using Application.Features.Coupons.Quries.GetAllPublicCoupons.Dto;
using Application.Features.Dto;
using Application.Messaging.Query;
using Domain.Shared;

namespace Application.Features.Coupons.Quries.GetAllPublicCoupons;

public record GetAllPublicCouponsQuery(
    string? Search,
    int Page,
    int PageSize)
    : IQuery<Result<PaginatedVm<CouponLookupDto>>>;
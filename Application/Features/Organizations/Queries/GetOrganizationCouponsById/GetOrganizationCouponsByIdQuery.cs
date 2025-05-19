using Application.Features.Dto;
using Application.Features.Organizations.Queries.GetOrganizationCouponsById.Dto;
using Application.Messaging.Query;
using Domain.Shared;

namespace Application.Features.Organizations.Queries.GetOrganizationCouponsById;

public record GetOrganizationCouponsByIdQuery(
    Guid Id,
    string? Search,
    int Page = 0,
    int PageSize = 10) : IQuery<Result<PaginatedVm<CouponLookupDto>>>;
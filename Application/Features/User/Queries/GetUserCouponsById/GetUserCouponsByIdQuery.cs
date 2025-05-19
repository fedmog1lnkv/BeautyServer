using Application.Features.Dto;
using Application.Features.User.Queries.GetUserCouponsById.Dto;
using Application.Messaging.Query;
using Domain.Shared;

namespace Application.Features.User.Queries.GetUserCouponsById;

public record GetUserCouponsByIdQuery(
    Guid Id,
    string? Search,
    int Page = 0,
    int PageSize = 10) : IQuery<Result<PaginatedVm<CouponLookupDto>>>;
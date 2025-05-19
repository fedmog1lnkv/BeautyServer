namespace Application.Features.User.Queries.GetUserCouponsById.Dto;

public sealed class CouponLookupDto
{
    public required Guid Id { get; init; }
    public required Guid OrganizationId { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string Code { get; init; }
    public required string DiscountType { get; init; }
    public required decimal DiscountValue { get; init; }
    public required bool IsPublic { get; init; }
    public required int UsageLimit { get; init; }
    public required DateOnly StartDate { get; init; }
    public required DateOnly EndDate { get; init; }
}
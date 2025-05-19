using Domain.Enums;
using Domain.Errors;
using Domain.Primitives;
using Domain.Repositories.Organizations;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entities;

public sealed class Coupon : AggregateRoot, IAuditableEntity
{
    private readonly List<User> _users = [];
    private readonly List<Service> _services = [];

    private Coupon(
        Guid id,
        Guid organizationId,
        CouponName name,
        CouponDescription description,
        CouponCode code,
        CouponDiscountType discountType,
        CouponDiscountValue discountValue,
        bool isPublic,
        CouponUsageLimit usageLimit,
        DateOnly startDate,
        DateOnly endDate,
        DateTime createdOnUtc) : base(id)
    {
        OrganizationId = organizationId;
        Name = name;
        Description = description;
        Code = code;
        DiscountType = discountType;
        DiscountValue = discountValue;
        IsPublic = isPublic;
        UsageLimit = usageLimit;
        StartDate = startDate;
        EndDate = endDate;
        CreatedOnUtc = createdOnUtc;
    }

#pragma warning disable CS8618
    private Coupon() { }
#pragma warning restore CS8618

    public Guid OrganizationId { get; private set; }
    public CouponName Name { get; private set; }
    public CouponDescription Description { get; private set; }
    public CouponCode Code { get; private set; }
    public CouponDiscountType DiscountType { get; private set; }
    public CouponDiscountValue DiscountValue { get; private set; }
    public bool IsPublic { get; private set; }
    public CouponUsageLimit UsageLimit { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    public Organization Organization { get; private set; } = null!;

    public IReadOnlyCollection<User> Users => _users.AsReadOnly();
    public IReadOnlyCollection<Service> Services => _services.AsReadOnly();
    
    public bool IsUniversalCoupon => !_services.Any();

    public static async Task<Result<Coupon>> Create(
        Guid id,
        Guid organizationId,
        string name,
        string description,
        string code,
        CouponDiscountType discountType,
        decimal discountValue,
        bool isPublic,
        int usageLimit,
        DateOnly startDate,
        DateOnly endDate,
        DateTime createdOnUtc,
        IOrganizationReadOnlyRepository organizationRepository,
        CancellationToken cancellationToken)
    {
        if (startDate > endDate)
            return Result.Failure<Coupon>(DomainErrors.Coupon.DateRangeInvalid);

        var organizationExists = await organizationRepository.ExistsAsync(organizationId, cancellationToken);
        if (!organizationExists)
            return Result.Failure<Coupon>(DomainErrors.Organization.NotFound(organizationId));

        var nameResult = CouponName.Create(name);
        if (nameResult.IsFailure)
            return Result.Failure<Coupon>(nameResult.Error);

        var descriptionResult = CouponDescription.Create(description);
        if (descriptionResult.IsFailure)
            return Result.Failure<Coupon>(descriptionResult.Error);

        var codeResult = CouponCode.Create(code);
        if (codeResult.IsFailure)
            return Result.Failure<Coupon>(codeResult.Error);

        var discountValueResult = CouponDiscountValue.Create(discountValue);
        if (discountValueResult.IsFailure)
            return Result.Failure<Coupon>(discountValueResult.Error);

        var usageLimitResult = CouponUsageLimit.Create(usageLimit, usageLimit);
        if (usageLimitResult.IsFailure)
            return Result.Failure<Coupon>(usageLimitResult.Error);

        return Result.Success(
            new Coupon(
                id,
                organizationId,
                nameResult.Value,
                descriptionResult.Value,
                codeResult.Value,
                discountType,
                discountValueResult.Value,
                isPublic,
                usageLimitResult.Value,
                startDate,
                endDate,
                createdOnUtc));
    }

    public Result UseOnce()
    {
        if (UsageLimit.Remaining <= 0)
            return Result.Failure(DomainErrors.CouponUsageLimit.NoRemainingUses);

        var usageLimitResult = CouponUsageLimit.Create(UsageLimit.Total, UsageLimit.Remaining - 1);
        if (usageLimitResult.IsFailure)
            return Result.Failure(usageLimitResult.Error);

        UsageLimit = usageLimitResult.Value;
        return Result.Success();
    }

    public decimal ApplyDiscount(decimal originalPrice)
    {
        if (originalPrice <= 0)
            return originalPrice;

        var discountedPrice = originalPrice;

        switch (DiscountType)
        {
            case CouponDiscountType.Fixed:
                discountedPrice = originalPrice - DiscountValue.Value;
                break;

            case CouponDiscountType.Percentage:
                discountedPrice = originalPrice * (1 - DiscountValue.Value / 100m);
                break;

            default:
                break;
        }

        if (discountedPrice < 0)
            discountedPrice = 0;

        return decimal.Round(discountedPrice, 2);
    }
    
    public Result AddServices(IEnumerable<Service> services)
    {
        foreach (var service in services)
        {
            if (service.OrganizationId != OrganizationId)
                return Result.Failure(DomainErrors.Service.NotFound(service.Id));

            if (!_services.Contains(service))
                _services.Add(service);
        }

        ModifiedOnUtc = DateTime.UtcNow;
        return Result.Success();
    }
}
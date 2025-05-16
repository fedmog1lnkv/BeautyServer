using Domain.Enums;
using Domain.Errors;
using Domain.Shared;

namespace Domain.Extensions;

public static class OrganizationSubscriptionExtensions
{
    public static Result<OrganizationSubscription> ToEnum(this string value)
    {
        return Enum.TryParse<OrganizationSubscription>(value, true, out var result)
            ? Result.Success(result)
            : Result.Failure<OrganizationSubscription>(DomainErrors.OrganizationSubscription.NotFound(value));
    }
}
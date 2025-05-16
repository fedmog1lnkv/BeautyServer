using Domain.Enums;
using Domain.Errors;
using Domain.Shared;

namespace Domain.Extensions;

public static class StaffRoleExtension 
{
    public static Result<StaffRole> ToEnum(this string value)
    {
        return Enum.TryParse<StaffRole>(value, true, out var result)
            ? Result.Success(result)
            : Result.Failure<StaffRole>(DomainErrors.StaffRole.NotFound(value));
    }
}
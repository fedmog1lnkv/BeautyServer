using Domain.Enums;

namespace Domain.Extensions;

public static class UserRoleExtensions
{
    private static readonly Dictionary<UserRole, Guid> _roleIds = new()
    {
        {
            UserRole.Admin,
            Guid.Parse("e75c3e48-b40b-4f50-a3a5-6c347d35c89e")
        },
        {
            UserRole.User,
            Guid.Parse("d2fb5f72-c850-4b96-a1c3-94b36f8a2c9f")
        }
    };

    public static Guid ToGuid(this UserRole role)
    {
        if (_roleIds.TryGetValue(role, out var id))
            return id;
        throw new KeyNotFoundException($"Role {role} does not have an associated ID.");
    }
}
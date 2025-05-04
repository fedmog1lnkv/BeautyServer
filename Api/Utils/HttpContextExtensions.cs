namespace Api.Utils;

public static class HttpContextExtensions
{
    public static Guid GetUserId(this HttpContext context)
    {
        return Guid.TryParse(context.Items["user_id"]?.ToString(), out var staffId) ? staffId : Guid.Empty;
    }

    public static bool IsAdmin(this HttpContext context)
    {
        if (!context.Items.TryGetValue("is_admin", out var isAdmin))
            return false;
        return isAdmin switch
        {
            bool adminBool => adminBool,
            string adminStr when bool.TryParse(adminStr, out var parsed) => parsed,
            _ => false
        };
    }

    public static Guid GetStaffId(this HttpContext context)
    {
        return Guid.TryParse(context.Items["staff_id"]?.ToString(), out var staffId) ? staffId : Guid.Empty;
    }

    public static Guid GetStaffOrganizationId(this HttpContext context)
    {
        return Guid.TryParse(context.Items["organization_id"]?.ToString(), out var organizationId) ? organizationId
            : Guid.Empty;
    }

    public static bool IsManager(this HttpContext context)
    {
        if (!context.Items.TryGetValue("is_manager", out var isManager))
            return false;
        return isManager switch
        {
            bool managerBool => managerBool,
            string managerStr when bool.TryParse(managerStr, out var parsed) => parsed,
            _ => false
        };
    }
}
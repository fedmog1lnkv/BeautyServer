namespace Api.Utils;

public static class HttpContextExtensions
{
    public static Guid GetUserId(this HttpContext context)
    {
        return Guid.Parse(context.Items["user_id"]?.ToString() ?? string.Empty);
    }

    public static bool IsAdmin(this HttpContext context)
    {
        return context.Items.TryGetValue("is_admin", out var isAdmin) && isAdmin is bool admin && admin;
    }
    
    public static Guid GetStaffId(this HttpContext context)
    {
        return Guid.Parse(context.Items["staff_id"]?.ToString() ?? string.Empty);
    }
    
    public static bool IsManager(this HttpContext context)
    {
        return context.Items.TryGetValue("is_manager", out var isManager) && isManager is bool manager && manager;
    }
}
using Application.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;

namespace Api.Filters;

public class StaffValidationFilter: ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // TODO : add check expired

        var tokenString = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (string.IsNullOrEmpty(tokenString))
        {
            context.Result = new UnauthorizedObjectResult(new { error = "Missing Authorization header" });
            return;
        }

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(tokenString);

            var staffId = jwtToken.Claims.FirstOrDefault(c => c.Type == "staff_id")?.Value;
            var organizationId = jwtToken.Claims.FirstOrDefault(c => c.Type == "organization_id")?.Value;
            var isManager = jwtToken.Claims.FirstOrDefault(c => c.Type == "is_manager")?.Value;

            bool isManagerBool = false;
            if (isManager != null && bool.TryParse(isManager, out var isManagerParsed))
            {
                isManagerBool = isManagerParsed;
            }
            if (Guid.TryParse(staffId, out var staffGuid) && Guid.TryParse(organizationId, out var organizationGuid))
            {
                context.HttpContext.Items["staff_id"] = staffGuid;
                context.HttpContext.Items["organization_id"] = organizationGuid;
                context.HttpContext.Items["is_manager"] = isManagerBool;
            }
            else
            {
                context.Result =
                    new UnauthorizedObjectResult(new { error = "Invalid token, missing or invalid claims." });
                return;
            }
        }
        catch (Exception ex)
        {
            context.Result = new UnauthorizedObjectResult(
                new
                {
                    error = "Invalid or expired token, you need to use refresh token or authorize again",
                    details = ex.Message
                });
        }

        base.OnActionExecuting(context);
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;

namespace Api.Filters;

public class UserValidationFilter : ActionFilterAttribute
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

            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value;
            var isAdmin = jwtToken.Claims.FirstOrDefault(c => c.Type == "is_admin")?.Value;

            var isAdminHeader = context.HttpContext.Request.Headers["IsAdmin"].FirstOrDefault();
            if (isAdminHeader != "True")
                isAdmin = "false";

            context.HttpContext.Items["user_id"] = userId;
            context.HttpContext.Items["is_admin"] = isAdmin;
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
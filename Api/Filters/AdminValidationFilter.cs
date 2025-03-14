using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;

namespace Api.Filters;

public class AdminValidationFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var tokenString = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        // Проверка на наличие токена
        if (string.IsNullOrEmpty(tokenString))
        {
            context.Result = new UnauthorizedObjectResult(new { error = "Missing Authorization header" });
            return;
        }

        try
        {
            // Разбираем токен
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(tokenString);

            // Извлекаем нужные данные из токена
            var isAdmin = jwtToken.Claims.FirstOrDefault(c => c.Type == "is_admin")?.Value;

            // Если пользователь не администратор, возвращаем ошибку
            if (isAdmin != "true")
            {
                context.Result = new UnauthorizedObjectResult(new { error = "Access denied: admin privileges required" });
                return;
            }

            // Сохраняем данные в HttpContext.Items для дальнейшего использования в контроллерах
            context.HttpContext.Items["is_admin"] = true;

            // Переход к следующему фильтру
            base.OnActionExecuting(context);
        }
        catch (Exception ex)
        {
            // Ошибка при парсинге токена
            context.Result = new UnauthorizedObjectResult(new
            {
                error = "Invalid or expired token, you need to use refresh token or authorize again",
                details = ex.Message
            });
        }
    }
}
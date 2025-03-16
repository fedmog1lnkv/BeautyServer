using Application.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Authentication.User;

public class UserJwtProvider(IOptions<JwtOptions> options) : IUserJwtProvider
{
    private readonly JwtOptions _options = options.Value;

    public string GenerateToken(Guid id, bool isAdmin)
    {
        var claims = new Claim[]
        {
            new(UserClaims.UserId, id.ToString()),
            new(UserClaims.IsAdmin, isAdmin.ToString())
        };

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            claims,
            null,
            DateTime.UtcNow.AddMinutes(15),
            signingCredentials);

        var tokenValue = new JwtSecurityTokenHandler()
            .WriteToken(token);

        return tokenValue;
    }

    public string GenerateRefreshToken(Guid id, bool isAdmin)
    {
        var claims = new Claim[]
        {
            new(UserClaims.UserId, id.ToString()),
            new(UserClaims.IsAdmin, isAdmin.ToString())
        };

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            claims,
            null,
            DateTime.UtcNow.AddDays(30),
            signingCredentials);

        var tokenValue = new JwtSecurityTokenHandler()
            .WriteToken(token);

        return tokenValue;
    }

    public (Guid UserId, bool IsAdmin)? ParseUserToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _options.Issuer,
                ValidAudience = _options.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey))
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            if (validatedToken is not JwtSecurityToken jwtToken)
                return null;

            var userIdClaim = principal.FindFirst(UserClaims.UserId);
            var isAdminClaim = principal.FindFirst(UserClaims.IsAdmin);

            if (userIdClaim == null || isAdminClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                return null;

            var isAdmin = bool.TryParse(isAdminClaim.Value, out var parsedIsAdmin) && parsedIsAdmin;

            return (userId, isAdmin);
        }
        catch
        {
            return null;
        }
    }
}
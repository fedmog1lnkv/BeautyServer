using Application.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Authentication.Staff;

public class StaffJwtProvider(IOptions<JwtOptions> options) : IStaffJwtProvider
{
    private readonly JwtOptions _options = options.Value;

    public string GenerateToken(
        Guid staffId,
        Guid organizationId,
        bool isManager)
    {
        var claims = new Claim[]
        {
            new(StaffClaims.StaffId, staffId.ToString()),
            new(StaffClaims.OrganizationId, organizationId.ToString()),
            new(StaffClaims.IsManager, isManager.ToString())
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

        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
        return tokenValue;
    }

    // Генерация refresh токена для сотрудников
    public string GenerateRefreshToken(
        Guid staffId,
        Guid organizationId,
        bool isManager)
    {
        var claims = new Claim[]
        {
            new(StaffClaims.StaffId, staffId.ToString()),
            new(StaffClaims.OrganizationId, organizationId.ToString()),
            new(StaffClaims.IsManager, isManager.ToString())
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

        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
        return tokenValue;
    }

    public (Guid StaffId, string OrganizationId, bool IsManager)? ParseStaffToken(string token)
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

            var staffIdClaim = principal.FindFirst(StaffClaims.StaffId);
            var organizationIdClaim = principal.FindFirst(StaffClaims.OrganizationId);
            var isManagerClaim = principal.FindFirst(StaffClaims.IsManager);

            if (staffIdClaim == null || organizationIdClaim == null || isManagerClaim == null ||
                !Guid.TryParse(staffIdClaim.Value, out var staffId))
                return null;

            var organizationId = organizationIdClaim.Value;
            var isManager = bool.TryParse(isManagerClaim.Value, out var parsedIsManager) && parsedIsManager;

            return (staffId, organizationId, isManager);
        }
        catch
        {
            return null;
        }
    }
}
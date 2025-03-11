using Application.Abstractions;
using Microsoft.Extensions.Options;

namespace Infrastructure.Authentication;

public sealed class JwtProvider(IOptions<JwtOptions> options) : IJwtProvider
{
    private readonly JwtOptions _options = options.Value;

    // public string Generate(User user)
    // {
    //     var claims = new Claim[]
    //     {
    //         new(CustomClaims.UserId, user.Id.ToString())
    //     };
    //
    //     var signingCredentials = new SigningCredentials(
    //         new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
    //         SecurityAlgorithms.HmacSha256);
    //
    //     var token = new JwtSecurityToken(
    //         _options.Issuer,
    //         _options.Audience,
    //         claims,
    //         null,
    //         DateTime.UtcNow.AddYears(1),
    //         signingCredentials);
    //
    //     string tokenValue = new JwtSecurityTokenHandler()
    //         .WriteToken(token);
    //
    //     return tokenValue;
    // }
}
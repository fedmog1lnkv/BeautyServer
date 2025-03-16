namespace Application.Abstractions;

public interface IUserJwtProvider : IJwtProvider
{
    string GenerateToken(Guid id, bool isAdmin);
    string GenerateRefreshToken(Guid id, bool isAdmin);
    (Guid UserId, bool IsAdmin)? ParseUserToken(string token);
}
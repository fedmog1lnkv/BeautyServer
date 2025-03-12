﻿namespace Application.Abstractions;

public interface IJwtProvider
{
    string GenerateToken(Guid id, bool isAdmin);
    string GenerateRefreshToken(Guid id, bool isAdmin);
    (Guid UserId, bool IsAdmin)? ParseToken(string token);
}
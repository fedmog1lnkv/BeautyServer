using Application.Abstractions;
using Application.Features.User.Commands.RefreshToken.Dto;
using Application.Messaging.Command;
using Domain.Errors;
using Domain.Repositories.Users;
using Domain.Shared;

namespace Application.Features.User.Commands.RefreshToken;

public class RefreshTokenCommandHandler(
    IUserJwtProvider jwtProvider,
    IUserRepository userRepository)
    : ICommandHandler<RefreshTokenCommand, Result<TokensVm>>
{
    public async Task<Result<TokensVm>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var claims = jwtProvider.ParseUserToken(request.RefreshToken);
        if (claims == null)
        {
            return Result.Failure<TokensVm>(DomainErrors.User.InvalidRefreshToken);
        }

        if (!Guid.TryParse(claims.Value.UserId.ToString(), out var userId))
        {
            return Result.Failure<TokensVm>(DomainErrors.User.InvalidUserIdRefreshToken);
        }

        var user = await userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return Result.Failure<TokensVm>(DomainErrors.User.NotFound(userId));
        }

        var accessToken = jwtProvider.GenerateToken(user.Id, claims.Value.IsAdmin);
        var newRefreshToken = jwtProvider.GenerateRefreshToken(user.Id, claims.Value.IsAdmin);

        return Result.Success<TokensVm>(
            new TokensVm
            {
                Token = accessToken,
                RefreshToken = newRefreshToken
            });
    }
}
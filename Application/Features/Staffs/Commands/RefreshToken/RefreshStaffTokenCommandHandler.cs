using Application.Abstractions;
using Application.Features.Staffs.Commands.RefreshToken.Dto;
using Application.Messaging.Command;
using Domain.Enums;
using Domain.Errors;
using Domain.Repositories.Staffs;
using Domain.Shared;

namespace Application.Features.Staffs.Commands.RefreshToken;

public class RefreshStaffTokenCommandHandler(
    IStaffJwtProvider jwtProvider,
    IStaffReadOnlyRepository staffReadOnlyRepository)
    : ICommandHandler<RefreshStaffTokenCommand, Result<TokensVm>>
{
    public async Task<Result<TokensVm>> Handle(RefreshStaffTokenCommand request, CancellationToken cancellationToken)
    {
        var claims = jwtProvider.ParseStaffToken(request.RefreshToken);
        if (claims == null)
            return Result.Failure<TokensVm>(DomainErrors.User.InvalidRefreshToken);

        if (!Guid.TryParse(claims.Value.StaffId.ToString(), out var userId))
        {
            return Result.Failure<TokensVm>(DomainErrors.Staff.InvalidStaffIdRefreshToken);
        }

        var staff = await staffReadOnlyRepository.GetByIdAsync(userId, cancellationToken);
        if (staff == null)
        {
            return Result.Failure<TokensVm>(DomainErrors.User.NotFound(userId));
        }

        var accessToken = jwtProvider.GenerateToken(
            staff.Id,
            staff.OrganizationId,
            staff.Role == StaffRole.Manager);
        
        var newRefreshToken = jwtProvider.GenerateRefreshToken(
            staff.Id,
            staff.OrganizationId,
            staff.Role == StaffRole.Manager);

        return Result.Success<TokensVm>(
            new TokensVm
            {
                Token = accessToken,
                RefreshToken = newRefreshToken
            });
    }
}
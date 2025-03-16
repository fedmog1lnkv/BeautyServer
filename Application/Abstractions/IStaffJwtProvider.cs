namespace Application.Abstractions;

public interface IStaffJwtProvider : IJwtProvider
{
    string GenerateToken(
        Guid staffId,
        Guid organizationId,
        bool isManager);

    string GenerateRefreshToken(
        Guid staffId,
        Guid organizationId,
        bool isManager);

    (Guid StaffId, string OrganizationId, bool IsManager)? ParseStaffToken(string token);
}
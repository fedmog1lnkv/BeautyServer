using Application.Common.Mappings;
using Application.Features.Staffs.Commands.RefreshToken;
using Application.Features.User.Commands.RefreshToken;
using AutoMapper;

namespace Api.Controllers.Staff.Models;

public class StaffRefreshTokenDto : IMapWith<RefreshStaffTokenCommand>
{
    public string RefreshToken { get; set; } = string.Empty;

    public static void Mapping(Profile profile) =>
        profile
            .CreateMap<StaffRefreshTokenDto, RefreshStaffTokenCommand>()
            .ForMember(x => x.RefreshToken, opt => opt.MapFrom(y => y.RefreshToken));
}
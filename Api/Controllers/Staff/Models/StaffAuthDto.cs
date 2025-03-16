using Application.Common.Mappings;
using Application.Features.Staffs.Commands.Auth;
using Application.Features.User.Commands.Auth;
using AutoMapper;

namespace Api.Controllers.Staff.Models;

public class StaffAuthDto : IMapWith<AuthStaffCommand>
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;

    public static void Mapping(Profile profile) =>
        profile
            .CreateMap<StaffAuthDto, AuthStaffCommand>()
            .ForMember(x => x.PhoneNumber, opt => opt.MapFrom(y => y.PhoneNumber))
            .ForMember(x => x.Code, opt => opt.MapFrom(y => y.Code));
}
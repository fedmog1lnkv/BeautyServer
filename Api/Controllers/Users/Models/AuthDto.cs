using Application.Common.Mappings;
using Application.Features.User.Commands.Auth;
using AutoMapper;

namespace Api.Controllers.Users.Models;

public class AuthDto : IMapWith<AuthCommand>
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;

    public static void Mapping(Profile profile) =>
        profile
            .CreateMap<AuthDto, AuthCommand>()
            .ForMember(x => x.PhoneNumber, opt => opt.MapFrom(y => y.PhoneNumber))
            .ForMember(x => x.Code, opt => opt.MapFrom(y => y.Code));
}
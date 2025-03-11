using Application.Common.Mappings;
using Application.Features.Authorization.Commands.Register;
using AutoMapper;

namespace Api.Controllers.Authorization.Models;

public class RegisterDto : IMapWith<RegisterCommand>
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public static void Mapping(Profile profile) =>
        profile.CreateMap<RegisterDto, RegisterCommand>()
            .ForMember(x => x.Username, opt => opt.MapFrom(y => y.Username))
            .ForMember(x => x.Password, opt => opt.MapFrom(y => y.Password));
}
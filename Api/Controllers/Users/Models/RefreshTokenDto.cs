using Application.Common.Mappings;
using Application.Features.User.Commands.RefreshToken;
using AutoMapper;

namespace Api.Controllers.Users.Models;

public class RefreshTokenDto : IMapWith<RefreshTokenCommand>
{
    public string RefreshToken { get; set; } = string.Empty;

    public static void Mapping(Profile profile) =>
        profile
            .CreateMap<RefreshTokenDto, RefreshTokenCommand>()
            .ForMember(x => x.RefreshToken, opt => opt.MapFrom(y => y.RefreshToken));
}
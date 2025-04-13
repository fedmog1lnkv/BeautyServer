using Application.Common.Mappings;
using Application.Features.User.Commands.UpdateUser;
using AutoMapper;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers.Users.Models;

public class FirebaseTokenDto : IMapWith<UpdateUserCommand>
{
    [SwaggerIgnore]
    public Guid Id { get; set; }
    public required string Token { get; set; }

    public static void Mapping(Profile profile)
    {
        profile.CreateMap<FirebaseTokenDto, UpdateUserCommand>()
            .ForMember(x => x.Id, opt => opt.MapFrom(y => y.Id))
            .ForMember(x => x.FirebaseToken, opt => opt.MapFrom(y => y.Token))
            .ConstructUsing(src => new UpdateUserCommand(src.Id, null, src.Token, null, null, null));
    }
}
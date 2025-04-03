using Application.Common.Mappings;
using Application.Features.User.Commands.UpdateUser;
using AutoMapper;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers.Users.Models;

public class UpdateUserDto : IMapWith<UpdateUserCommand>
{
    [SwaggerIgnore]
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public static void Mapping(Profile profile) =>
        profile
            .CreateMap<UpdateUserDto, UpdateUserCommand>()
            .ForMember(x => x.Id, opt => opt.MapFrom(y => y.Id))
            .ForMember(x => x.Name, opt => opt.MapFrom(y => y.Name));
}
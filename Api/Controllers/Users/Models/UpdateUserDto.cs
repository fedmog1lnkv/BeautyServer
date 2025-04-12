using Application.Common.Mappings;
using Application.Features.User.Commands.UpdateUser;
using AutoMapper;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers.Users.Models;

public class UpdateUserDto : IMapWith<UpdateUserCommand>
{
    [SwaggerIgnore]
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public bool? ReceiveOrderNotifications { get; set; }

    public bool? ReceivePromoNotifications { get; set; }

    public static void Mapping(Profile profile) =>
        profile
            .CreateMap<UpdateUserDto, UpdateUserCommand>()
            .ForMember(x => x.Id, opt => opt.MapFrom(y => y.Id))
            .ForMember(x => x.Name, opt => opt.MapFrom(y => y.Name))
            .ForMember(x => x.ReceiveOrderNotifications, opt => opt.MapFrom(y => y.ReceiveOrderNotifications))
            .ForMember(x => x.ReceivePromoNotifications, opt => opt.MapFrom(y => y.ReceivePromoNotifications));
}
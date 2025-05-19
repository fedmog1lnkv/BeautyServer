using Application.Common.Mappings;
using Application.Features.User.Commands.AddCouponToUser;
using AutoMapper;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers.Users.Models;

public class AddCouponToUserDto : IMapWith<AddCouponToUserCommand>
{
    [SwaggerIgnore]
    public Guid UserId { get; set; }
    public required string CouponCode { get; set; }

    public static void Mapping(Profile profile) =>
        profile.CreateMap<AddCouponToUserDto, AddCouponToUserCommand>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.CouponCode, opt => opt.MapFrom(src => src.CouponCode));
}
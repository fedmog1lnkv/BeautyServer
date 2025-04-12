using Application.Common.Mappings;
using AutoMapper;
using Domain.ValueObjects;

namespace Api.Controllers.Users.Models;

public class UserSettingsVm : IMapWith<UserSettings>
{
    public bool ReceiveOrderNotifications { get; set; }
    public bool ReceivePromoNotifications { get; set; }

    public void Mapping(Profile profile) =>
        profile.CreateMap<UserSettings, UserSettingsVm>()
            .ForMember(dest => dest.ReceiveOrderNotifications, opt => opt.MapFrom(src => src.ReceiveOrderNotifications))
            .ForMember(
                dest => dest.ReceivePromoNotifications,
                opt => opt.MapFrom(src => src.ReceivePromoNotifications));
}
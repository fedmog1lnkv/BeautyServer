using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;

namespace Api.Controllers.Users.Models;

public class UserVm : IMapWith<User>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string? Photo { get; set; }
    public UserSettingsVm Settings { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<User, UserVm>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.Value))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber.Value))
            .ForMember(
                dest => dest.Photo,
                opt => opt.MapFrom(
                    src => src.Photo != null ? src.Photo.Value :
                        null))
            .ForMember(dest => dest.Settings, opt => opt.MapFrom(src => src.Settings));
    }
}
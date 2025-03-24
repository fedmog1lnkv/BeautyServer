using Application.Common.Mappings;
using AutoMapper;

namespace Api.Controllers.Venue.Models;

public class VenueLookupDto : IMapWith<Domain.Entities.Venue>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ThemeVm Theme { get; set; } = new ThemeVm();
    public LocationVm Location { get; set; } = new LocationVm();

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Domain.Entities.Venue, VenueLookupDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.Value))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description.Value))
            .ForMember(
                dest => dest.Theme,
                opt => opt.MapFrom(
                    src => new ThemeVm
                    {
                        Color = src.Theme.Color,
                        Photo = src.Theme.Photo
                    }))
            .ForMember(
                dest => dest.Location,
                opt => opt.MapFrom(
                    src => new LocationVm
                    {
                        Latitude = src.Location.Latitude,
                        Longitude = src.Location.Longitude
                    }));
    }
}
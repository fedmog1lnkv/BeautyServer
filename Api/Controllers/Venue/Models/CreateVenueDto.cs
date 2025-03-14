using Application.Common.Mappings;
using Application.Features.Venues.Commands.CreateVenue;
using AutoMapper;

namespace Api.Controllers.Venue.Models;

public class CreateVenueDto : IMapWith<CreateVenueCommand>
{
    public Guid OrganizationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Color { get; set; }
    public string? Photo { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<CreateVenueDto, CreateVenueCommand>()
            .ForMember(dest => dest.OrganizationId, opt => opt.MapFrom(src => src.OrganizationId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Color))
            .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => src.Photo))
            .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Latitude))
            .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Longitude));
    }
}
using Application.Common.Mappings;
using Application.Features.Venues.Commands.UpdateVenue;
using AutoMapper;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers.Venue.Models;

public class UpdateVenueDto : IMapWith<UpdateVenueCommand>
{
    [SwaggerIgnore]
    public Guid InitiatorId { get; set; }
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Address { get; set; }
    public string? Color { get; set; }
    public string? Photo { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public List<Guid>? ServiceIds { get; set; }
    public List<Guid>? Photos { get; set; }

    public void Mapping(Profile profile) =>
        profile.CreateMap<UpdateVenueDto, UpdateVenueCommand>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.InitiatorId, opt => opt.MapFrom(src => src.InitiatorId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Color))
            .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => src.Photo))
            .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Latitude))
            .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Longitude))
            .ForMember(dest => dest.ServiceIds, opt => opt.MapFrom(src => src.ServiceIds))
            .ForMember(dest => dest.Photos, opt => opt.MapFrom(src => src.Photos));
}
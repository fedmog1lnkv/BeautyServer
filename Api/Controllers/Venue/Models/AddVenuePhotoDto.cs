using Application.Common.Mappings;
using Application.Features.Venues.Commands.AddVenuePhoto;
using AutoMapper;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers.Venue.Models;

public class AddVenuePhotoDto : IMapWith<AddVenuePhotoCommand>
{
    [SwaggerIgnore]
    public Guid StaffId { get; set; }
    public Guid VenueId { get; set; }
    public string Photo { get; set; }

    public void Mapping(Profile profile) =>
        profile.CreateMap<AddVenuePhotoDto, AddVenuePhotoCommand>()
            .ForMember(dest => dest.StaffId, opt => opt.MapFrom(src => src.StaffId))
            .ForMember(dest => dest.VenueId, opt => opt.MapFrom(src => src.VenueId))
            .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => src.Photo));
}
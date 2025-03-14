using Application.Common.Mappings;
using Application.Features.Organizations.Commands.CreateOrganization;
using AutoMapper;

namespace Api.Controllers.Organization.Models;

public class CreateOrganizationDto : IMapWith<CreateOrganizationCommand>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Color { get; set; }
    public string? Photo { get; set; }

    public static void Mapping(Profile profile) =>
        profile.CreateMap<CreateOrganizationDto, CreateOrganizationCommand>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Color))
            .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => src.Photo));
}
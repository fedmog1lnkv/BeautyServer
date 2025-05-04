using Application.Common.Mappings;
using Application.Features.Services.Commands.CreateService;
using AutoMapper;

namespace Api.Controllers.Services.Models;

public class CreateServiceDto : IMapWith<CreateServiceCommand>
{
    public Guid OrganizationId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int? Duration { get; set; }
    public double? Price { get; set; }

    public void Mapping(Profile profile) =>
        profile.CreateMap<CreateServiceDto, CreateServiceCommand>()
            .ForMember(
                dest => dest.OrganizationId,
                opt => opt.MapFrom(src => src.OrganizationId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price));
}
using Application.Common.Mappings;
using AutoMapper;

namespace Api.Controllers.Organization.Models;

public class OrganizationVm : IMapWith<Domain.Entities.Organization>
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string Subscription { get; set; } 
    public ThemeVm Theme { get; set; } = new();

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Domain.Entities.Organization, OrganizationVm>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.Value))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description.Value))
            .ForMember(dest => dest.Subscription, opt => opt.MapFrom(src => src.Subscription.ToString()))
            .ForMember(
                dest => dest.Theme,
                opt => opt.MapFrom(
                    src => new ThemeVm
                    {
                        Color = src.Theme.Color,
                        Photo = src.Theme.Photo
                    }));
    }
}
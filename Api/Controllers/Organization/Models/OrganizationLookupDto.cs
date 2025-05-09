using Application.Common.Mappings;
using AutoMapper;

namespace Api.Controllers.Organization.Models;

public class OrganizationLookupDto : IMapWith<Domain.Entities.Organization>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Subscription { get; set; } = string.Empty;
    public ThemeVm Theme { get; set; } = new ThemeVm();

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Domain.Entities.Organization, OrganizationLookupDto>()
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
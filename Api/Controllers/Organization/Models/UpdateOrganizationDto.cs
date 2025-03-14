using Application.Common.Mappings;
using Application.Features.Organizations.Commands.UpdateOrganization;
using AutoMapper;

namespace Api.Controllers.Organization.Models;

public class UpdateOrganizationDto : IMapWith<UpdateOrganizationCommand>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Subscription { get; set; }
    public string? Color { get; set; }
    public string? Photo { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<UpdateOrganizationCommand, UpdateOrganizationDto>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
            .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Name))
            .ForMember(d => d.Description, opt => opt.MapFrom(s => s.Description))
            .ForMember(d => d.Subscription, opt => opt.MapFrom(s => s.Subscription))
            .ForMember(d => d.Color, opt => opt.MapFrom(s => s.Color))
            .ForMember(d => d.Photo, opt => opt.MapFrom(s => s.Photo));
    }
}
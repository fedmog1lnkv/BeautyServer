
using Api.Controllers.Staffs.Models;
using Api.Controllers.Venue.Models;
using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;

namespace Api.Controllers.Services.Models;

public class ServiceVm : IMapWith<Service>
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public TimeSpan? Duration { get; set; }
    public decimal? Price { get; set; }
    public string Photo { get; set; }
    public List<Guid> StaffIds { get; set; }
    public List<Guid> VenueIds { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Service, ServiceVm>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
            .ForMember(d => d.OrganizationId, opt => opt.MapFrom(s => s.OrganizationId))
            .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Name.Value))
            .ForMember(
                d => d.Description,
                opt => opt.MapFrom(s => s.Description.Value))
            .ForMember(d => d.Duration, opt => opt.MapFrom(s => s.Duration))
            .ForMember(d => d.Price, opt => opt.MapFrom(s => s.Price.Value))
            .ForMember(d => d.Photo, opt => opt.MapFrom(s => s.Photo.Value))
            .ForMember(d => d.StaffIds, opt => opt.MapFrom(s => s.Staffs.Select(s => s.Id).ToList()))
            .ForMember(d => d.VenueIds, opt => opt.MapFrom(s => s.Venues.Select(v => v.Id).ToList()));
    }
}
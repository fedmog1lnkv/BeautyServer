using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;

namespace Api.Controllers.Venue.Models;

public class VenueStaffWithServicesLookupDto : IMapWith<Staff>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public List<Guid> Services { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Staff, VenueStaffWithServicesLookupDto>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
            .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Name.Value))
            .ForMember(d => d.PhoneNumber, opt => opt.MapFrom(s => s.PhoneNumber.Value))
            .ForMember(d => d.Services, opt => opt.MapFrom(s => s.Services.Select(ser => ser.Id).ToList()));
    }
}
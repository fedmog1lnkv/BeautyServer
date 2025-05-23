using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;

namespace Api.Controllers.Staffs.Models;

public class StaffVm : IMapWith<Staff>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string Role { get; set; }
    public double Rating { get; set; }
    public string? Photo { get; set; }
    public List<Guid> Services { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Staff, StaffVm>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
            .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Name.Value))
            .ForMember(d => d.PhoneNumber, opt => opt.MapFrom(s => s.PhoneNumber.Value))
            .ForMember(d => d.Role, opt => opt.MapFrom(s => s.Role.ToString()))
            .ForMember(d => d.Rating, opt => opt.MapFrom(s => s.Rating.Value))
            .ForMember(
                d => d.Photo,
                opt => opt.MapFrom(
                    src => src.Photo != null ? src.Photo.Value :
                        null))
            .ForMember(d => d.Services, opt => opt.MapFrom(s => s.Services.Select(ser => ser.Id).ToList()));
    }
}
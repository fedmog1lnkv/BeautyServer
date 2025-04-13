using Application.Common.Mappings;
using Application.Features.Services.Commands.UpdateService;
using AutoMapper;

namespace Api.Controllers.Services.Models;

public class UpdateServiceDto : IMapWith<UpdateServiceCommand>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? Duration { get; set; }
    public double? Price { get; set; }
    public string? Photo { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<UpdateServiceDto, UpdateServiceCommand>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
            .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Name))
            .ForMember(d => d.Description, opt => opt.MapFrom(s => s.Description))
            .ForMember(d => d.Duration, opt => opt.MapFrom(s => s.Duration))
            .ForMember(d => d.Price, opt => opt.MapFrom(s => s.Price))
            .ForMember(d => d.Photo, opt => opt.MapFrom(s => s.Photo));
    }
}
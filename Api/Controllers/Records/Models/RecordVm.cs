using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;

namespace Api.Controllers.Records.Models;

public class RecordVm : IMapWith<Record>
{
    public Guid Id { get; set; }
    public RecordVmStaffLookupDto Staff { get; set; }
    public RecordVmUserLookupDto User { get; set; }
    public RecordVmServiceLookupDto Service { get; set; }
    public RecordVmVenueLookupDto Venue { get; set; }
    public string Comment { get; set; }
    public string Status { get; set; }
    public DateTimeOffset StartTimestamp { get; set; }
    public DateTimeOffset EndTimestamp { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Record, RecordVm>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Staff, opt => opt.MapFrom(src => src.Staff))
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
            .ForMember(dest => dest.Service, opt => opt.MapFrom(src => src.Service))
            .ForMember(dest => dest.Venue, opt => opt.MapFrom(src => src.Venue))
            .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.StartTimestamp, opt => opt.MapFrom(src => src.StartTimestamp))
            .ForMember(dest => dest.EndTimestamp, opt => opt.MapFrom(src => src.EndTimestamp));
    }
}

public class RecordVmUserLookupDto : IMapWith<Staff>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string Photo { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Staff, RecordVmUserLookupDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.Value))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber.Value))
            .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => src.Photo.Value));
    }
}

public class RecordVmStaffLookupDto : IMapWith<Staff>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string Photo { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Staff, RecordVmStaffLookupDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.Value))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber.Value))
            .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => src.Photo.Value));
    }
}

public class RecordVmServiceLookupDto : IMapWith<Service>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public TimeSpan? Duration { get; set; }
    public double? Price { get; set; }
    public string? Photo { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Service, RecordVmServiceLookupDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.Value))
            .ForMember(
                dest => dest.Description,
                opt => opt.MapFrom(src => src.Description != null ? src.Description.Value : null))
            .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration))
            .ForMember(
                dest => dest.Price,
                opt => opt.MapFrom(src => src.Price != null ? src.Price.Value : (double?)null)).ForMember(
                dest => dest.Photo,
                opt => opt.MapFrom(
                    src => src.Photo != null ? src.Photo.Value :
                        null));
    }
}

public class RecordVmVenueLookupDto : IMapWith<Domain.Entities.Venue>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public LocationVm Location { get; set; }
    public ThemeVm Theme { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Domain.Entities.Venue, RecordVmVenueLookupDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.Value))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description.Value))
            .ForMember(
                dest => dest.Location,
                opt => opt.MapFrom(
                    src => new LocationVm
                    {
                        Latitude = src.Location.Latitude,
                        Longitude = src.Location.Longitude
                    }))
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

public class LocationVm
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public class ThemeVm
{
    public string Color { get; set; } = string.Empty;
    public string? Photo { get; set; }
}
using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;
using Domain.ValueObjects;

namespace Api.Controllers.Staffs.Models;

public class TimeSlotsVm : IMapWith<TimeSlot>
{
    public Guid Id { get; set; }
    public DateOnly Date { get; set; }
    public List<IntervalsVm> Intervals { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<TimeSlot, TimeSlotsVm>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
            .ForMember(d => d.Date, opt => opt.MapFrom(s => s.Date))
            .ForMember(d => d.Intervals, opt => opt.MapFrom(s => s.Intervals));
    }
}

public class IntervalsVm : IMapWith<Interval>
{
    public TimeSpan Start { get; set; }
    public TimeSpan End { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Interval, IntervalsVm>()
            .ForMember(d => d.Start, opt => opt.MapFrom(s => s.Start))
            .ForMember(d => d.End, opt => opt.MapFrom(s => s.End));
    }
}
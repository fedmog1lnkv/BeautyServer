using Application.Common.Mappings;
using Application.Features.Staffs.Commands.AddTimeSlot;
using Application.Features.Staffs.Commands.AddTimeSlot.Dto;
using AutoMapper;

namespace Api.Controllers.Staffs.Models;

public class TimeSlotDto : IMapWith<AddTimeSlotCommand>
{
    public Guid StaffId { get; set; }
    public Guid VenueId { get; set; }
    public DateOnly Date { get; set; }
    public List<IntervalsDto> Intervals { get; set; } = [];

    public void Mapping(Profile profile)
    {
        profile.CreateMap<TimeSlotDto, AddTimeSlotCommand>()
            .ForMember(d => d.StaffId, opt => opt.MapFrom(s => s.StaffId))
            .ForMember(d => d.VenueId, opt => opt.MapFrom(s => s.VenueId))
            .ForMember(d => d.Date, opt => opt.MapFrom(s => s.Date))
            .ForMember(d => d.Intervals, opt => opt.MapFrom(s => s.Intervals));
    }
}

using Application.Common.Mappings;
using Application.Features.Staffs.Commands.AddTimeSlot;
using Application.Features.Staffs.Commands.AddTimeSlot.Dto;
using AutoMapper;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers.Staffs.Models;

public class TimeSlotDto : IMapWith<AddTimeSlotCommand>
{
    [SwaggerIgnore]
    public Guid InitiatorId { get; set; }
    public Guid StaffId { get; set; }
    public Guid VenueId { get; set; }
    public DateOnly Date { get; set; }
    public List<IntervalsDto> Intervals { get; set; } = [];

    public void Mapping(Profile profile)
    {
        profile.CreateMap<TimeSlotDto, AddTimeSlotCommand>()
            .ForMember(d => d.InitiatorId, opt => opt.MapFrom(s => s.InitiatorId))
            .ForMember(d => d.StaffId, opt => opt.MapFrom(s => s.StaffId))
            .ForMember(d => d.VenueId, opt => opt.MapFrom(s => s.VenueId))
            .ForMember(d => d.Date, opt => opt.MapFrom(s => s.Date))
            .ForMember(d => d.Intervals, opt => opt.MapFrom(s => s.Intervals));
    }
}
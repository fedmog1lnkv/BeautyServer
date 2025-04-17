using Application.Common.Mappings;
using Application.Features.Staffs.Commands.UpdateTimeSlot;
using Application.Features.Staffs.Commands.UpdateTimeSlot.Dto;
using AutoMapper;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers.Staffs.Models;

public class UpdateTimeSlotDto : IMapWith<UpdateTimeSlotCommand>
{
    [SwaggerIgnore]
    public Guid InitiatorId { get; set; }
    public Guid StaffId { get; set; }
    public Guid TimeSlotId { get; set; }
    public List<UpdateIntervalsDto> Intervals { get; set; } = [];

    public void Mapping(Profile profile)
    {
        profile.CreateMap<UpdateTimeSlotDto, UpdateTimeSlotCommand>()
            .ForMember(d => d.InitiatorId, opt => opt.MapFrom(s => s.InitiatorId))
            .ForMember(d => d.StaffId, opt => opt.MapFrom(s => s.StaffId))
            .ForMember(d => d.TimeSlotId, opt => opt.MapFrom(s => s.TimeSlotId))
            .ForMember(d => d.Intervals, opt => opt.MapFrom(s => s.Intervals));
    }
}
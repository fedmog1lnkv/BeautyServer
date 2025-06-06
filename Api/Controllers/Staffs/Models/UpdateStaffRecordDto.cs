using Application.Common.Mappings;
using Application.Features.Staffs.Commands.UpdateStaffRecord;
using AutoMapper;
using Domain.Enums;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers.Staffs.Models;

public class UpdateStaffRecordDto : IMapWith<UpdateStaffRecordCommand>
{
    [SwaggerIgnore]
    public Guid InitiatorId { get; set; }

    public Guid RecordId { get; set; }
    public string? Status { get; set; }

    public DateTime? StartTimestamp { get; set; }

    public DateTime? EndTimeStamp { get; set; }

    public void Mapping(Profile profile) =>
        profile.CreateMap<UpdateStaffRecordDto, UpdateStaffRecordCommand>()
            .ForMember(dest => dest.InitiatorId, opt => opt.MapFrom(src => src.InitiatorId))
            .ForMember(dest => dest.RecordId, opt => opt.MapFrom(src => src.RecordId))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.StartTimestamp, opt => opt.MapFrom(src => src.StartTimestamp))
            .ForMember(dest => dest.EndTimeStamp, opt => opt.MapFrom(src => src.EndTimeStamp));
}
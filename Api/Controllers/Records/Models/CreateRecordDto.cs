using Application.Common.Mappings;
using Application.Features.Records.Commands.CreateRecord;
using AutoMapper;

namespace Api.Controllers.Records.Models;

public class CreateRecordDto : IMapWith<CreateRecordCommand>
{
    public Guid? UserId { get; set; }
    public Guid StaffId { get; set; }
    public Guid ServiceId { get; set; }
    public DateTime StartTimestamp { get; set; }
    public DateTime? EndTimeStamp { get; set; }

    public void Mapping(Profile profile) =>
        profile.CreateMap<CreateRecordDto, CreateRecordCommand>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.StaffId, opt => opt.MapFrom(src => src.StaffId))
            .ForMember(dest => dest.ServiceId, opt => opt.MapFrom(src => src.ServiceId))
            .ForMember(dest => dest.StartTimestamp, opt => opt.MapFrom(src => src.StartTimestamp))
            .ForMember(dest => dest.EndTimeStamp, opt => opt.MapFrom(src => src.EndTimeStamp));
}
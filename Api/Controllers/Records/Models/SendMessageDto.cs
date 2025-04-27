using Application.Common.Mappings;
using Application.Features.Records.Commands.AddMessage;
using AutoMapper;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers.Records.Models;

public class SendMessageDto : IMapWith<AddMessageCommand>
{
    [SwaggerIgnore]
    public Guid SenderId { get; set; }
    public Guid RecordId { get; set; }
    public required string Text { get; set; }

    public void Mapping(Profile profile) =>
        profile.CreateMap<SendMessageDto, AddMessageCommand>()
            .ForMember(dest => dest.SenderId, opt => opt.MapFrom(src => src.SenderId))
            .ForMember(dest => dest.RecordId, opt => opt.MapFrom(src => src.RecordId))
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text));
}
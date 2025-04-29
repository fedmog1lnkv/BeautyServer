using Application.Common.Mappings;
using Application.Features.Records.Commands.MarkAsReadMessage;
using AutoMapper;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers.Records.Models;

public class MessagesDto : IMapWith<MarkAsReadMessageCommand>
{
    [SwaggerIgnore]
    public Guid ReaderId { get; set; }
    [SwaggerIgnore]
    public Guid RecordId { get; set; }
    public required List<Guid> MessageIds { get; set; }

    public void Mapping(Profile profile) =>
        profile.CreateMap<MessagesDto, MarkAsReadMessageCommand>()
            .ForMember(dest => dest.ReaderId, opt => opt.MapFrom(src => src.ReaderId))
            .ForMember(dest => dest.RecordId, opt => opt.MapFrom(src => src.RecordId))
            .ForMember(dest => dest.MessageIds, opt => opt.MapFrom(src => src.MessageIds));
}
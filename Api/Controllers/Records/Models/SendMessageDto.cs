using Application.Common.Mappings;
using Application.Features.Records.Commands.AddMessage;
using AutoMapper;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers.Records.Models;

public class SendMessageDto : IMapWith<AddMessageCommand>
{
    [SwaggerIgnore]
    public Guid SenderId { get; set; }
    [SwaggerIgnore]
    public Guid RecordId { get; set; }
    public Guid? MessageId { get; set; }
    public required string Text { get; set; }

    public void Mapping(Profile profile) =>
        profile.CreateMap<SendMessageDto, AddMessageCommand>()
            .ForMember(dest => dest.SenderId, opt => opt.MapFrom(src => src.SenderId))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.MessageId))
            .ForMember(dest => dest.RecordId, opt => opt.MapFrom(src => src.RecordId))
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
            .ConstructUsing(
                src =>
                    MessageId.HasValue
                        ? new AddMessageCommand(src.MessageId!.Value, src.RecordId, src.SenderId, src.Text)
                        : new AddMessageCommand(src.RecordId, src.SenderId, src.Text));
}
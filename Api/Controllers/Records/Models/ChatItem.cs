using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;

namespace Api.Controllers.Records.Models;

public class ChatItem : IMapWith<RecordMessage>
{
    public required Guid Id { get; set; }
    public Guid? SenderId { get; set; }
    public required string Type { get; set; }
    public required string Text { get; set; }
    public bool? IsRead { get; set; }
    public required DateTime CreatedAt { get; set; }
    public DateTime? ReadAt { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<RecordMessage, ChatItem>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.SenderId, opt => opt.MapFrom(src => src.SenderId))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "Message"))
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Message.Value))
            .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => src.IsRead))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.ReadAt, opt => opt.MapFrom(src => src.ReadAt));

        profile.CreateMap<RecordStatusLog, ChatItem>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.SenderId, opt => opt.MapFrom(src => (Guid?)null))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.StatusChange.ToString()))
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Description.Value))
            .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => (bool?)null))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.Timestamp))
            .ForMember(dest => dest.ReadAt, opt => opt.MapFrom(src => (DateTime?)null));
    }
}
using Application.Common.Mappings;
using Application.Features.User.Commands.UpdateUserRecord;
using AutoMapper;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers.Users.Models;

public class UpdateUserRecordDto : IMapWith<UpdateUserRecordCommand>
{
    [SwaggerIgnore]
    public Guid UserId { get; set; }
    public Guid RecordId { get; set; }
    public string? Status { get; set; }

    public void Mapping(Profile profile) =>
        profile.CreateMap<UpdateUserRecordDto, UpdateUserRecordCommand>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.RecordId, opt => opt.MapFrom(src => src.RecordId))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));
}
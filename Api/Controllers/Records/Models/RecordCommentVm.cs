using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;
using Domain.ValueObjects;

namespace Api.Controllers.Records.Models;

public class RecordCommentVm : IMapWith<Record>
{
    public Guid RecordId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Record, RecordCommentVm>()
            .ForMember(dest => dest.RecordId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Review.Rating))
            .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Review.Comment));
    }
}

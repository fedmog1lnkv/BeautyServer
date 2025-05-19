using Application.Common.Mappings;
using Application.Features.Coupons.Commands.CreateCoupon;
using AutoMapper;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers.Coupons.Models;

public class CreateCouponDto : IMapWith<CreateCouponCommand>
{
    [SwaggerIgnore]
    public Guid InitiatorId { get; set; }
    public Guid OrganizationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DiscountType { get; set; } = string.Empty;
    public decimal DiscountValue { get; set; }
    public bool IsPublic { get; set; }
    public int UsageLimit { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public List<Guid>? ServiceIds { get; set; }

    public static void Mapping(Profile profile) =>
        profile.CreateMap<CreateCouponDto, CreateCouponCommand>()
            .ForMember(dest => dest.InitiatorId, opt => opt.MapFrom(src => src.InitiatorId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.DiscountType, opt => opt.MapFrom(src => src.DiscountType))
            .ForMember(dest => dest.DiscountValue, opt => opt.MapFrom(src => src.DiscountValue))
            .ForMember(dest => dest.IsPublic, opt => opt.MapFrom(src => src.IsPublic))
            .ForMember(dest => dest.UsageLimit, opt => opt.MapFrom(src => src.UsageLimit))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
            .ForMember(dest => dest.OrganizationId, opt => opt.MapFrom(src => src.OrganizationId))
            .ForMember(dest => dest.ServiceIds, opt => opt.MapFrom(src => src.ServiceIds));
}
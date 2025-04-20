using Application.Common.Mappings;
using Application.Features.Staffs.Commands.UpdateStaff;
using AutoMapper;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers.Staffs.Models;

public class FirebaseTokenDto : IMapWith<UpdateStaffCommand>
{
    [SwaggerIgnore]
    public Guid Id { get; set; }
    public required string Token { get; set; }

    public static void Mapping(Profile profile)
    {
        profile.CreateMap<FirebaseTokenDto, UpdateStaffCommand>()
            .ForMember(x => x.InitiatorId, opt => opt.MapFrom(y => y.Id))
            .ForMember(x => x.StaffId, opt => opt.MapFrom(y => y.Id))
            .ForMember(x => x.FirebaseToken, opt => opt.MapFrom(y => y.Token))
            .ConstructUsing(src => new UpdateStaffCommand(src.Id, src.Id, null, null, src.Token, null));
    }
}
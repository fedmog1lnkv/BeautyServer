using Application.Common.Mappings;
using Application.Features.Staffs.Commands.UpdateStaff;
using AutoMapper;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers.Staffs.Models;

public class UpdateStaffDto : IMapWith<UpdateStaffCommand>
{
    [SwaggerIgnore]
    public Guid InitiatorId { get; set; }
    public Guid StaffId { get; set; }
    public string? Name { get; set; }
    public string? Photo { get; set; }

    public static void Mapping(Profile profile) =>
        profile
            .CreateMap<UpdateStaffDto, UpdateStaffCommand>()
            .ForMember(x => x.InitiatorId, opt => opt.MapFrom(y => y.InitiatorId))
            .ForMember(x => x.StaffId, opt => opt.MapFrom(y => y.StaffId))
            .ForMember(x => x.Name, opt => opt.MapFrom(y => y.Name))
            .ForMember(x => x.Photo, opt => opt.MapFrom(y => y.Photo));
}
using Application.Common.Mappings;
using Application.Features.Staffs.Commands.UpdateStaff;
using AutoMapper;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers.Staffs.Models;

public class UpdateStaffDto : IMapWith<UpdateStaffCommand>
{
    [SwaggerIgnore]
    public Guid InitiatorId { get; set; }
    public required Guid StaffId { get; set; }
    public string? Name { get; set; }
    public string? Role { get; set; }
    public string? Photo { get; set; }
    public List<Guid>? ServiceIds { get; set; }

    public static void Mapping(Profile profile) =>
        profile
            .CreateMap<UpdateStaffDto, UpdateStaffCommand>()
            .ForMember(x => x.InitiatorId, opt => opt.MapFrom(y => y.InitiatorId))
            .ForMember(x => x.StaffId, opt => opt.MapFrom(y => y.StaffId))
            .ForMember(x => x.Name, opt => opt.MapFrom(y => y.Name))
            .ForMember(x => x.Role, opt => opt.MapFrom(y => y.Role))
            .ForMember(x => x.Photo, opt => opt.MapFrom(y => y.Photo))
            .ForMember(dest => dest.ServiceIds, opt => opt.MapFrom(src => src.ServiceIds))
            .ConstructUsing(
                src => new UpdateStaffCommand(
                    src.InitiatorId,
                    src.StaffId,
                    src.Name,
                    src.Role,
                    src.Photo,
                    null,
                    src.ServiceIds));
}
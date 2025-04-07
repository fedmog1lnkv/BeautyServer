using Application.Common.Mappings;
using Application.Features.Staffs.Commands.UpdateStaffRecord;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers.Staffs.Models;

public class UpdateStaffRecordDto : IMapWith<UpdateStaffRecordCommand>
{
    [SwaggerIgnore]
    public Guid InitiatorId { get; set; }

    public Guid Id { get; set; }
    public string? Status { get; set; }
    public string? Comment { get; set; }
}
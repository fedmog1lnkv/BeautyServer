using Api.Controllers.Base;
using Api.Controllers.Records.Models;
using Api.Filters;
using Application.Features.Records.Commands.CreateRecord;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Records;

[Route("api/record")]
public class RecordController(IMapper mapper) : BaseController
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [UserValidationFilter]
    public async Task<IActionResult> Create([FromBody] CreateRecordDto request)
    {
        var userId = Guid.Parse(HttpContext.Items["user_id"]?.ToString() ?? string.Empty);
        if (userId == Guid.Empty)
            return Unauthorized();

        if (request.UserId is not null && request.UserId != userId)
            return Unauthorized();

        request.UserId ??= userId;

        var command = mapper.Map<CreateRecordCommand>(request);

        var result = await Sender.Send(command);

        return result.IsFailure
            ? HandleFailure(result)
            : NoContent();
    }
    
    // get record/{id}
}
using Api.Controllers.Base;
using Api.Controllers.Records.Models;
using Api.Filters;
using Api.Utils;
using Application.Features.Records.Commands.CreateRecord;
using Application.Features.Records.Queries.GetRecordById;
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
        var userId = HttpContext.GetUserId();
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetRecordByIdQuery(id);

        var result = await Sender.Send(query);
        if (result.IsFailure)
            return HandleFailure(result);
        
        var record = mapper.Map<RecordVm>(result.Value);

        return Ok(record);
    }
}
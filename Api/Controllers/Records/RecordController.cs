using Api.Controllers.Base;
using Api.Controllers.Records.Models;
using Api.Filters;
using Api.Utils;
using Application.Features.Records.Commands.AddMessage;
using Application.Features.Records.Commands.CreateRecord;
using Application.Features.Records.Commands.DeleteMessage;
using Application.Features.Records.Queries.GetRecordById;
using Application.Features.Records.Queries.GetRecordMessagesById;
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

    [HttpPost("message")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [UserValidationFilter]
    [StaffValidationFilter]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageDto request)
    {
        var userId = HttpContext.GetUserId();
        var staffId = HttpContext.GetStaffId();
        if (userId == Guid.Empty && staffId == Guid.Empty)
            return Unauthorized();

        request.SenderId = userId == Guid.Empty ? staffId : userId;

        var command = mapper.Map<AddMessageCommand>(request);

        var result = await Sender.Send(command);

        return result.IsFailure
            ? HandleFailure(result)
            : NoContent();
    }

    [HttpGet("{id}/messages")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [UserValidationFilter]
    [StaffValidationFilter]
    public async Task<IActionResult> GetAllByRecord(Guid id)
    {
        var userId = HttpContext.GetUserId();
        var staffId = HttpContext.GetStaffId();
        if (userId == Guid.Empty && staffId == Guid.Empty)
            return Unauthorized();

        var initiatorId = userId == Guid.Empty ? staffId : userId;

        var query = new GetRecordMessagesByIdQuery(id, initiatorId);
        var result = await Sender.Send(query);

        return result.IsFailure
            ? HandleFailure(result)
            : NoContent();
    }
    
    [HttpDelete("{recordId}/message/{messageId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [UserValidationFilter]
    [StaffValidationFilter]
    public async Task<IActionResult> SendMessage(Guid recordId, Guid messageId)
    {
        var userId = HttpContext.GetUserId();
        var staffId = HttpContext.GetStaffId();
        if (userId == Guid.Empty && staffId == Guid.Empty)
            return Unauthorized();

        var initiatorId = userId == Guid.Empty ? staffId : userId;

        var command = new DeleteMessageCommand(initiatorId, recordId, messageId);

        var result = await Sender.Send(command);

        return result.IsFailure
            ? HandleFailure(result)
            : NoContent();
    }
}
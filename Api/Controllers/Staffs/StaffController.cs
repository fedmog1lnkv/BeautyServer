using Api.Controllers.Base;
using Api.Controllers.Staffs.Models;
using Api.Filters;
using Application.Features.Records.Queries.GetRecordsByStaffId;
using Application.Features.Staffs.Commands.AddTimeSlot;
using Application.Features.Staffs.Commands.Auth;
using Application.Features.Staffs.Commands.GeneratePhoneChallenge;
using Application.Features.Staffs.Commands.RefreshToken;
using Application.Features.Staffs.Queries.GetStaffWithServicesById;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TimeSlotDto = Api.Controllers.Staffs.Models.TimeSlotDto;

namespace Api.Controllers.Staffs;

[Route("api/staff")]
public class StaffController(IMapper mapper) : BaseController
{
    [HttpPost("phone_challenge")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> PhoneChallenge([FromBody] StaffPhoneChallengeDto request)
    {
        var command = mapper.Map<GenerateStaffPhoneChallengeCommand>(request);

        var result = await Sender.Send(command);

        return result.IsFailure
            ? HandleFailure(result)
            : NoContent();
    }

    [HttpPost("auth")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Auth([FromBody] StaffAuthDto request)
    {
        var command = mapper.Map<AuthStaffCommand>(request);

        var result = await Sender.Send(command);

        return result.IsFailure
            ? HandleFailure(result)
            : Ok(result.Value);
    }

    [HttpPost("refresh_token")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RefreshToken([FromBody] StaffRefreshTokenDto request)
    {
        var command = mapper.Map<RefreshStaffTokenCommand>(request);

        var result = await Sender.Send(command);

        return result.IsFailure
            ? HandleFailure(result)
            : Ok(result.Value);
    }

    [HttpPost("scheldue")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddTimeSlot([FromBody] TimeSlotDto request)
    {
        var command = mapper.Map<AddTimeSlotCommand>(request);

        var result = await Sender.Send(command);

        return result.IsFailure
            ? HandleFailure(result)
            : Ok();
    }

    [HttpGet("records")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [StaffValidationFilter]
    public async Task<IActionResult> GetRecords(
        [FromQuery] int limit,
        [FromQuery] int offset,
        [FromQuery] bool isPending)
    {
        if (limit <= 0 || offset < 0)
        {
            return BadRequest("Limit must be greater than zero, and offset cannot be negative.");
        }

        var staffId = Guid.Parse(HttpContext.Items["staff_id"]?.ToString() ?? string.Empty);
        if (staffId == Guid.Empty)
            return Unauthorized();
        
        var query = new GetRecordsByStaffIdQuery(staffId, limit, offset, isPending);

        var result = await Sender.Send(query);
        if (result.IsFailure)
            return HandleFailure(result);
// TODO : MAP
        // var venues = mapper.Map<List<StaffRecordLookupDto>>(result.Value);

        return Ok(result.Value);
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> GetById(Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest();
        
        var query = new GetStaffWithServicesByIdQuery(id);

        var result = await Sender.Send(query);
        if (result.IsFailure)
            return HandleFailure(result);
        
        var staff = mapper.Map<StaffVm>(result.Value);

        return Ok(staff);
    }
}
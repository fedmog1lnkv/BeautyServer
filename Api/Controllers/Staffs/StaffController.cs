using Api.Controllers.Base;
using Api.Controllers.Staffs.Models;
using Api.Filters;
using Api.Utils;
using Application.Features.Records.Queries.GetRecordsByStaffId;
using Application.Features.Staffs.Commands.AddTimeSlot;
using Application.Features.Staffs.Commands.Auth;
using Application.Features.Staffs.Commands.GeneratePhoneChallenge;
using Application.Features.Staffs.Commands.RefreshToken;
using Application.Features.Staffs.Commands.UpdateStaff;
using Application.Features.Staffs.Commands.UpdateStaffRecord;
using Application.Features.Staffs.Commands.UpdateTimeSlot;
using Application.Features.Staffs.Queries.GetStaffScheduleByIdAndDate;
using Application.Features.Staffs.Queries.GetStaffWithServicesById;
using Application.Features.Staffs.Queries.GetStaffWithTimeSlotsByIdAndVenueId;
using Application.Features.Staffs.Queries.GetStaffWorkloadById;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

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
    
    [HttpPost("firebase_token")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [StaffValidationFilter]
    public async Task<IActionResult> FirebaseToken([FromBody] FirebaseTokenDto request)
    {
        request.Id = HttpContext.GetUserId();

        var command = mapper.Map<UpdateStaffCommand>(request);

        var result = await Sender.Send(command);

        return result.IsFailure
            ? HandleFailure(result)
            : NoContent();
    }

    [HttpPost("schedule")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [StaffValidationFilter]
    public async Task<IActionResult> AddTimeSlot([FromBody] AddTimeSlotDto request)
    {
        request.InitiatorId = HttpContext.GetStaffId();
        var command = mapper.Map<AddTimeSlotCommand>(request);

        var result = await Sender.Send(command);

        return result.IsFailure
            ? HandleFailure(result)
            : Ok();
    }
    
    [HttpPatch("schedule")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [StaffValidationFilter]
    public async Task<IActionResult> UpdateTimeSlot([FromBody] UpdateTimeSlotDto request)
    {
        request.InitiatorId = HttpContext.GetStaffId();
        var command = mapper.Map<UpdateTimeSlotCommand>(request);

        var result = await Sender.Send(command);

        return result.IsFailure
            ? HandleFailure(result)
            : Ok();
    }

    [HttpGet("{id}/schedule")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTimeSlotByStaffId(Guid id, [FromQuery] Guid venueId)
    {
        var query = new GetStaffWithTimeSlotsByIdAndVenueIdQuery(id, venueId);

        var result = await Sender.Send(query);
        if (result.IsFailure)
            return HandleFailure(result);

        var timeSlots = mapper.Map<List<TimeSlotsVm>>(result.Value.TimeSlots);
        return Ok(timeSlots);
    }
    
    [HttpGet("schedule/workload/{year}/{month}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [StaffValidationFilter]
    public async Task<IActionResult> GetStaffWorkload(int year, int month, [FromQuery] Guid? staffId)
    {
        if (staffId is not null && !HttpContext.IsManager())
            return Unauthorized();
        
        staffId ??= HttpContext.GetStaffId();
        
        var query = new GetStaffWorkloadByIdQuery(staffId.Value, year, month);

        var result = await Sender.Send(query);
        if (result.IsFailure)
            return HandleFailure(result);

        return Ok(result.Value.Days);
    }
    
    [HttpGet("schedule/workload/{year}/{month}/{day}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [StaffValidationFilter]
    public async Task<IActionResult> GetStaffScheduleByDate(int year, int month, int day, [FromQuery] Guid? staffId)
    {
        if (staffId is not null && !HttpContext.IsManager())
            return Unauthorized();
        
        staffId ??= HttpContext.GetStaffId();
        
        var query = new GetStaffScheduleByIdAndDateQuery(staffId.Value, year, month, day);

        var result = await Sender.Send(query);
        if (result.IsFailure)
            return HandleFailure(result);

        return Ok(result.Value);
    }

    [HttpGet("records")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [StaffValidationFilter]
    public async Task<IActionResult> GetRecords(
        [FromQuery] DateOnly date,
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

        var query = new GetRecordsByStaffIdQuery(date, staffId, limit, offset, isPending);

        var result = await Sender.Send(query);
        if (result.IsFailure)
            return HandleFailure(result);

        var records = mapper.Map<List<StaffRecordsLookupDto>>(result.Value);

        return Ok(records);
    }
    
    [HttpPatch("record")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [StaffValidationFilter]
    public async Task<IActionResult> Update([FromBody] UpdateStaffRecordDto request)
    {
        request.InitiatorId = HttpContext.GetStaffId();
        var command = mapper.Map<UpdateStaffRecordCommand>(request);

        var result = await Sender.Send(command);

        return result.IsFailure
            ? HandleFailure(result)
            : NoContent();
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
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [StaffValidationFilter]
    public async Task<IActionResult> Get()
    {
        var id = HttpContext.GetStaffId();
        if (id == Guid.Empty)
            return BadRequest();

        var query = new GetStaffWithServicesByIdQuery(id);

        var result = await Sender.Send(query);
        if (result.IsFailure)
            return HandleFailure(result);

        var staff = mapper.Map<StaffVm>(result.Value);

        return Ok(staff);
    }

    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [StaffValidationFilter]
    public async Task<IActionResult> Update([FromBody] UpdateStaffDto request)
    {
        request.InitiatorId = HttpContext.GetStaffId();
        var command = mapper.Map<UpdateStaffCommand>(request);

        var result = await Sender.Send(command);

        return result.IsFailure
            ? HandleFailure(result)
            : NoContent();
    }
}
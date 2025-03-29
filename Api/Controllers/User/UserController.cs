using Api.Controllers.Base;
using Api.Controllers.User.Models;
using Api.Filters;
using Application.Features.Records.Queries.GetRecordsByUserId;
using Application.Features.User.Commands.Auth;
using Application.Features.User.Commands.GeneratePhoneChallenge;
using Application.Features.User.Commands.RefreshToken;
using Application.Features.User.Queries.GetUserById;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.User;

[Route("api/user")]
public class UserController(IMapper mapper) : BaseController
{
    [HttpPost("phone_challenge")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> PhoneChallenge([FromBody] PhoneChallengeDto request)
    {
        var command = mapper.Map<GeneratePhoneChallengeCommand>(request);

        var result = await Sender.Send(command);

        return result.IsFailure
            ? HandleFailure(result)
            : NoContent();
    }

    [HttpPost("auth")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Auth([FromBody] AuthDto request)
    {
        var command = mapper.Map<AuthCommand>(request);

        var result = await Sender.Send(command);

        return result.IsFailure
            ? HandleFailure(result)
            : Ok(result.Value);
    }
    
    [HttpPost("refresh_token")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto request)
    {
        var command = mapper.Map<RefreshTokenCommand>(request);

        var result = await Sender.Send(command);

        return result.IsFailure
            ? HandleFailure(result)
            : Ok(result.Value);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [UserValidationFilter]
    public async Task<IActionResult> Get()
    {
        var userId = Guid.Parse(HttpContext.Items["user_id"]?.ToString() ?? string.Empty);
        if (userId == Guid.Empty)
            return Unauthorized();
        
        var query = new GetUserByIdQuery(userId);

        var result = await Sender.Send(query);
        if (result.IsFailure)
            return HandleFailure(result);
        
        var userVm = mapper.Map<UserVm>(result.Value);

        return Ok(userVm);
    }

    [HttpGet("records")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [UserValidationFilter]
    public async Task<IActionResult> GetRecords(
        [FromQuery] int limit,
        [FromQuery] int offset,
        [FromQuery] bool isPending)
    {
        if (limit <= 0 || offset < 0)
        {
            return BadRequest("Limit must be greater than zero, and offset cannot be negative.");
        }

        var userId = Guid.Parse(HttpContext.Items["user_id"]?.ToString() ?? string.Empty);
        if (userId == Guid.Empty)
            return Unauthorized();
        
        var query = new GetRecordsByUserIdQuery(userId, limit, offset, isPending);

        var result = await Sender.Send(query);
        if (result.IsFailure)
            return HandleFailure(result);
        
        var records = mapper.Map<List<UserRecordsLookupDto>>(result.Value);

        return Ok(records);
    }
    
    // TODO : update name
}
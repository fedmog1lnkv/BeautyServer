using Api.Controllers.Base;
using Api.Controllers.Staff.Models;
using Application.Features.Staffs.Commands.Auth;
using Application.Features.Staffs.Commands.GeneratePhoneChallenge;
using Application.Features.Staffs.Commands.RefreshToken;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Staff;

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
}
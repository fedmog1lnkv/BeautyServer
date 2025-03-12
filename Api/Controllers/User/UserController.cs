using Api.Controllers.Base;
using Api.Controllers.User.Models;
using Application.Features.User.Commands.Auth;
using Application.Features.User.Commands.GeneratePhoneChallenge;
using Application.Features.User.Commands.RefreshToken;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.User;

public class UserController(IMapper mapper) : BaseController
{
    [HttpPost]
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

    [HttpPost]
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
    
    [HttpPost]
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
}
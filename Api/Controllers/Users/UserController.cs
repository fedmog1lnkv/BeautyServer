using Api.Controllers.Base;
using Api.Controllers.Users.Models;
using Api.Filters;
using Api.Utils;
using Application.Features.Records.Queries.GetRecordsByUserId;
using Application.Features.User.Commands.AddCouponToUser;
using Application.Features.User.Commands.Auth;
using Application.Features.User.Commands.GeneratePhoneChallenge;
using Application.Features.User.Commands.RefreshToken;
using Application.Features.User.Commands.UpdateUser;
using Application.Features.User.Commands.UpdateUserRecord;
using Application.Features.User.Queries.GetUserById;
using Application.Features.User.Queries.GetUserCouponsById;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Users;

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
    
    [HttpPost("firebase_token")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [UserValidationFilter]
    public async Task<IActionResult> FirebaseToken([FromBody] FirebaseTokenDto request)
    {
        request.Id = HttpContext.GetUserId();

        var command = mapper.Map<UpdateUserCommand>(request);

        var result = await Sender.Send(command);

        return result.IsFailure
            ? HandleFailure(result)
            : NoContent();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [UserValidationFilter]
    public async Task<IActionResult> Get()
    {
        var query = new GetUserByIdQuery(HttpContext.GetUserId());
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
        [FromQuery] int offset)
    {
        if (limit <= 0 || offset < 0)
            return BadRequest("Limit must be greater than zero, and offset cannot be negative.");

        var userId = HttpContext.GetUserId();
        var query = new GetRecordsByUserIdQuery(userId, limit, offset);

        var result = await Sender.Send(query);
        if (result.IsFailure)
            return HandleFailure(result);

        var records = mapper.Map<List<UserRecordsLookupDto>>(result.Value);

        return Ok(records);
    }

    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [UserValidationFilter]
    public async Task<IActionResult> Update([FromBody] UpdateUserDto request)
    {
        request.Id = HttpContext.GetUserId();

        var command = mapper.Map<UpdateUserCommand>(request);

        var result = await Sender.Send(command);

        return result.IsFailure
            ? HandleFailure(result)
            : NoContent();
    }
    
    [HttpPatch("record")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [UserValidationFilter]
    public async Task<IActionResult> Update([FromBody] UpdateUserRecordDto request)
    {
        request.UserId = HttpContext.GetUserId();
        var command = mapper.Map<UpdateUserRecordCommand>(request);

        var result = await Sender.Send(command);

        return result.IsFailure
            ? HandleFailure(result)
            : NoContent();
    }
    
    [HttpPost("coupons")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [UserValidationFilter]
    public async Task<IActionResult> AddCouponToUser([FromBody] AddCouponToUserDto request)
    {
        request.UserId = HttpContext.GetUserId();

        var command = mapper.Map<AddCouponToUserCommand>(request);
        var result = await Sender.Send(command);

        return result.IsFailure
            ? HandleFailure(result)
            : NoContent();
    }
    
    [HttpGet("coupons")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [UserValidationFilter]
    public async Task<IActionResult> GetUserCoupons(
        [FromQuery] string? search,
        [FromQuery] int page = 0,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetUserCouponsByIdQuery(HttpContext.GetUserId(), search, page, pageSize);
        var result = await Sender.Send(query);

        return result.IsFailure
            ? HandleFailure(result)
            : Ok(result.Value);
    }
}
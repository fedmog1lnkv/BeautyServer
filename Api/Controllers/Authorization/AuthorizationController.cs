using Api.Controllers.Authorization.Models;
using Api.Controllers.Base;
using Application.Features.Authorization.Commands.Register;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Api.Controllers.Authorization;

[Route("api/[controller]")]
[ApiController]
public class AuthorizationController(IMapper mapper) : BaseController
{
    /// <summary>
    /// Авторзиация пользователя
    /// </summary>
    /// <param name="request"></param>
    /// <responce code="200">Оператор успешно авторизовался на юните</responce>
    /// <responce code="409">Ошибка авторизации.</responce>
    /// <returns></returns>
    [HttpPost]
    [Route("Register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterDto request)
    {
        var command = mapper.Map<RegisterCommand>(request);

        var result = await Sender.Send(command);

        if (result.IsFailure)
            return HandleFailure(result);

        return Ok(result.Value);
    }

    /*/// <summary>
    /// Авторзиация пользователя
    /// </summary>
    /// <param name="request"></param>
    /// <responce code="200">Оператор успешно авторизовался на юните</responce>
    /// <responce code="409">Ошибка авторизации.</responce>
    /// <returns></returns>
    [HttpPost]
    [Route("Login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Login( [FromBody] LoginRequestVm request)
    {
        var command = mapper.Map<RequestCurrentUnitControlCommand>( request );

        var result = await Sender.Send( command );

        if (result.IsFailure)
            return HandleFailure( result );

        return Ok( result.Value );
    }

    /// <summary>
    /// Выход пользователя
    /// </summary>
    /// <param name="staffId"></param>
    /// <responce code="204">Успешный выход из системы.</responce>
    /// <responce code="409">Ошибка, авторизованный оператор не найден.</responce>
    /// <returns></returns>
    [HttpPost]
    [Route("Logout")]
    [ProducesResponseType( StatusCodes.Status204NoContent )]
    [ProducesResponseType( typeof( ProblemDetails ), StatusCodes.Status409Conflict )]
    public async Task<IActionResult> Logout( Guid staffId )
    {
        var command = new LogoutRequestUnitControlCommand( staffId );
        var result = await Sender.Send( command );

        if ( result.IsFailure )
            return HandleFailure( result );

        return NoContent();
    }*/
}
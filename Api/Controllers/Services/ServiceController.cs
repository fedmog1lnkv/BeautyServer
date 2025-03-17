using Api.Controllers.Base;
using Api.Controllers.Services.Models;
using Api.Filters;
using Application.Features.Services.Commands.CreateService;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Services;

[Route("api/service")]
public class ServiceController(IMapper mapper) : BaseController
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [StaffValidationFilter]
    public async Task<IActionResult> Create([FromBody] CreateServiceDto request)
    {
        var command = mapper.Map<CreateServiceCommand>(request);

        var result = await Sender.Send(command);

        return result.IsFailure
            ? HandleFailure(result)
            : NoContent();
    }
}
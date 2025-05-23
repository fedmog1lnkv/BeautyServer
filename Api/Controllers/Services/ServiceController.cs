using Api.Controllers.Base;
using Api.Controllers.Services.Models;
using Api.Controllers.Venue.Models;
using Api.Filters;
using Application.Features.Organizations.Queries.GetOrganizationServicesById.Dto;
using Application.Features.Services.Commands.CreateService;
using Application.Features.Services.Commands.DeleteService;
using Application.Features.Services.Commands.UpdateService;
using Application.Features.Services.Queries.GetServiceById;
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
    
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetServiceByIdQuery(id);
        var result = await Sender.Send(query);

        if (result.IsFailure)
            return HandleFailure(result);

        var dto = mapper.Map<ServiceVm>(result.Value);
        return Ok(dto);
    }
    
    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [StaffValidationFilter]
    public async Task<IActionResult> Update([FromBody] UpdateServiceDto request)
    {
        var command = mapper.Map<UpdateServiceCommand>(request);

        var result = await Sender.Send(command);

        return result.IsFailure
            ? HandleFailure(result)
            : NoContent();
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteServiceCommand(id);
        var result = await Sender.Send(command);

        if (result.IsFailure)
            return HandleFailure(result);

        return NoContent();
    }
}
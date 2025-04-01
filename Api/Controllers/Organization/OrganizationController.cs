using Api.Controllers.Base;
using Api.Controllers.Organization.Models;
using Api.Filters;
using Application.Features.Organizations.Commands.CreateOrganization;
using Application.Features.Organizations.Commands.DeleteOrganization;
using Application.Features.Organizations.Commands.UpdateOrganization;
using Application.Features.Organizations.Queries.GetAllOrganizations;
using Application.Features.Organizations.Queries.GetOrganiazationById;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Organization;

[Route("api/organization")]
public class OrganizationController(IMapper mapper) : BaseController
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [AdminValidationFilter]
    public async Task<IActionResult> Create([FromBody] CreateOrganizationDto request)
    {
        var command = mapper.Map<CreateOrganizationCommand>(request);

        var result = await Sender.Send(command);

        return result.IsFailure
            ? HandleFailure(result)
            : NoContent();
    }

    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [UserValidationFilter]
    public async Task<IActionResult> Update([FromBody] UpdateOrganizationDto request)
    {
        var isAdmin = HttpContext.Items["is_admin"] as string;
        if (isAdmin != "True")
            request.Subscription = null;

        var command = mapper.Map<UpdateOrganizationCommand>(request);

        var result = await Sender.Send(command);

        return result.IsFailure
            ? HandleFailure(result)
            : NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteOrganizationCommand(id);
        var result = await Sender.Send(command);

        if (result.IsFailure)
            return HandleFailure(result);

        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int limit,
        [FromQuery] int offset)
    {
        if (limit <= 0 || offset < 0)
            return BadRequest("Limit must be greater than zero, and offset cannot be negative.");

        var query = new GetAllOrganizationsQuery(limit, offset);
        var result = await Sender.Send(query);

        if (result.IsFailure)
            return HandleFailure(result);

        var organizations = mapper.Map<List<OrganizationLookupDto>>(result.Value);

        return Ok(organizations);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetOrganizationByIdQuery(id);
        var result = await Sender.Send(query);

        if (result.IsFailure)
            return HandleFailure(result);

        var venueDto = mapper.Map<OrganizationVm>(result.Value);
        return Ok(venueDto);
    }
}
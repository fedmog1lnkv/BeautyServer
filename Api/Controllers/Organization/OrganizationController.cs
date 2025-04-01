using Api.Controllers.Base;
using Api.Controllers.Organization.Models;
using Api.Filters;
using Application.Features.Organizations.Commands.CreateOrganization;
using Application.Features.Organizations.Commands.DeleteOrganization;
using Application.Features.Organizations.Commands.UpdateOrganization;
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
}
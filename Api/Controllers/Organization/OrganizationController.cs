using Api.Controllers.Base;
using Api.Controllers.Organization.Models;
using Api.Controllers.Venue.Models;
using Api.Filters;
using Api.Utils;
using Application.Features.Organizations.Commands.CreateOrganization;
using Application.Features.Organizations.Commands.DeleteOrganization;
using Application.Features.Organizations.Commands.UpdateOrganization;
using Application.Features.Organizations.Queries.GetAllOrganizations;
using Application.Features.Organizations.Queries.GetOrganiazationById;
using Application.Features.Organizations.Queries.GetOrganiazationVenuesById;
using Application.Features.Organizations.Queries.GetOrganizationCouponsById;
using Application.Features.Organizations.Queries.GetOrganizationServicesById;
using Application.Features.Organizations.Queries.GetOrganizationStaffsById;
using Application.Features.Organizations.Queries.GetOrganizationStatisticById;
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
    [StaffValidationFilter]
    [UserValidationFilter]
    public async Task<IActionResult> Update([FromBody] UpdateOrganizationDto request)
    {
        if (!HttpContext.IsManager() && !HttpContext.IsAdmin())
            return Unauthorized();

        if (HttpContext.IsAdmin())
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

    [HttpGet("statistic")]
    [StaffValidationFilter]
    public async Task<IActionResult> GetStatisticForOrganization()
    {
        if (!HttpContext.IsManager() && !HttpContext.IsAdmin())
            return Unauthorized();

        var query = new GetOrganizationStatisticByIdQuery(HttpContext.GetStaffOrganizationId());
        var result = await Sender.Send(query);

        return result.IsFailure
            ? HandleFailure(result)
            : Ok(result.Value);
    }

    [HttpGet("venues")]
    [StaffValidationFilter]
    public async Task<IActionResult> GetOrganizationVenues()
    {
        if (!HttpContext.IsManager() && !HttpContext.IsAdmin())
            return Unauthorized();

        var query = new GetOrganizationVenuesByIdQuery(HttpContext.GetStaffOrganizationId());
        var result = await Sender.Send(query);

        if (result.IsFailure)
            return HandleFailure(result);

        var response = mapper.Map<List<VenueLookupDto>>(result.Value);

        return result.IsFailure
            ? HandleFailure(result)
            : Ok(result.Value);
    }

    [HttpGet("services")]
    [StaffValidationFilter]
    public async Task<IActionResult> GetOrganizationServices(
        [FromQuery] string? search,
        [FromQuery] int page = 0,
        [FromQuery] int pageSize = 10)
    {
        if (!HttpContext.IsManager() && !HttpContext.IsAdmin())
            return Unauthorized();

        var organizationId = HttpContext.GetStaffOrganizationId();

        var query = new GetOrganizationServicesByIdQuery(organizationId, search, page, pageSize);
        var result = await Sender.Send(query);

        if (result.IsFailure)
            return HandleFailure(result);

        return Ok(result.Value);
    }

    [HttpGet("staffs")]
    [StaffValidationFilter]
    public async Task<IActionResult> GetOrganizationStaffs(
        [FromQuery] string? search,
        [FromQuery] int page = 0,
        [FromQuery] int pageSize = 10)
    {
        if (!HttpContext.IsManager() && !HttpContext.IsAdmin())
            return Unauthorized();

        var organizationId = HttpContext.GetStaffOrganizationId();

        var query = new GetOrganizationStaffsByIdQuery(organizationId, search, page, pageSize);
        var result = await Sender.Send(query);

        if (result.IsFailure)
            return HandleFailure(result);

        return Ok(result.Value);
    }

    [HttpGet("coupons")]
    [StaffValidationFilter]
    public async Task<IActionResult> GetOrganizationCoupons(
        [FromQuery] string? search,
        [FromQuery] int page = 0,
        [FromQuery] int pageSize = 10)
    {
        if (!HttpContext.IsManager() && !HttpContext.IsAdmin())
            return Unauthorized();

        var organizationId = HttpContext.GetStaffOrganizationId();

        var query = new GetOrganizationCouponsByIdQuery(organizationId, search, page, pageSize);
        var result = await Sender.Send(query);

        return result.IsFailure
            ? HandleFailure(result)
            : Ok(result.Value);
    }
}
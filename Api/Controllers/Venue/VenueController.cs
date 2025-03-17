using Api.Controllers.Base;
using Api.Controllers.Venue.Models;
using Api.Filters;
using Application.Features.Services.Queries.GetServicesByVenueId;
using Application.Features.Venues.Commands.CreateVenue;
using Application.Features.Venues.Commands.UpdateVenue;
using Application.Features.Venues.Queries.GetAllVenues;
using Application.Features.Venues.Queries.GetVenueById;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Venue;

[Route("api/venue")]
public class VenueController(IMapper mapper) : BaseController
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [StaffValidationFilter]
    public async Task<IActionResult> Create([FromBody] CreateVenueDto request)
    {
        var staffOrganizationId = HttpContext.Items["organization_id"] as Guid?;
        if (staffOrganizationId != request.OrganizationId)
            return Unauthorized("staffOrganizationId != request.OrganizationId");
        
        var isManager = HttpContext.Items["is_manager"] as bool?;
        if (!isManager!.Value)
            return Unauthorized("Not manager");
        
        var command = mapper.Map<CreateVenueCommand>(request);

        var result = await Sender.Send(command);

        return result.IsFailure
            ? HandleFailure(result)
            : NoContent();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [UserValidationFilter]
    public async Task<IActionResult> GetAll(
        [FromQuery] double? latitude,
        [FromQuery] double? longitude,
        [FromQuery] int limit,
        [FromQuery] int offset)
    {
        if (limit <= 0 || offset < 0)
        {
            return BadRequest("Limit must be greater than zero, and offset cannot be negative.");
        }

        var query = new GetAllVenuesQuery(limit, offset, latitude, longitude);

        var result = await Sender.Send(query);

        if (result.IsFailure)
            return HandleFailure(result);

        var venues = mapper.Map<List<VenueLookupDto>>(result.Value);

        return Ok(venues);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetVenueByIdQuery(id);
        var result = await Sender.Send(query);

        if (result.IsFailure)
            return HandleFailure(result);

        var venueDto = mapper.Map<VenueVm>(result.Value);
        return Ok(venueDto);
    }
    
    [HttpGet("{id}/services")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetServicesByVenueId(Guid id)
    {
        var query = new GetServicesByVenueIdQuery(id);
        var result = await Sender.Send(query);

        if (result.IsFailure)
            return HandleFailure(result);

        var venueDto = mapper.Map<List<VenueServiceVm>>(result.Value);
        return Ok(venueDto);
    }

    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [StaffValidationFilter]
    public async Task<IActionResult> Update([FromBody] UpdateVenueDto request)
    {
        var staffOrganizationId = HttpContext.Items["organization_id"] as Guid?;
        if (staffOrganizationId != request.OrganizationId)
            return Unauthorized();
        
        var isManager = HttpContext.Items["is_manager"] as bool?;
        if (!isManager!.Value)
            return Unauthorized();

        var command = mapper.Map<UpdateVenueCommand>(request);
        var result = await Sender.Send(command);

        return result.IsFailure
            ? HandleFailure(result)
            : Ok();
    }

    // [HttpPatch]
    // [ProducesResponseType(StatusCodes.Status204NoContent)]
    // [UserValidationFilter]
    // public async Task<IActionResult> Update([FromBody] UpdateOrganizationDto request)
    // {
    //     var isAdmin = HttpContext.Items["is_admin"] as string;
    //     if (isAdmin != "True")
    //         request.Subscription = null;
    //
    //     var command = mapper.Map<UpdateOrganizationCommand>(request);
    //
    //     var result = await Sender.Send(command);
    //
    //     return result.IsFailure
    //         ? HandleFailure(result)
    //         : NoContent();
    // }
}
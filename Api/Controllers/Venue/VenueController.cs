using Api.Controllers.Base;
using Api.Controllers.Venue.Models;
using Api.Filters;
using Application.Features.Venues.Commands.CreateVenue;
using Application.Features.Venues.Queries.GetAllVenues;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Venue;

[Route("api/venue")]
public class VenueController(IMapper mapper) : BaseController
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [AdminValidationFilter]
    public async Task<IActionResult> Create([FromBody] CreateVenueDto request)
    {
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
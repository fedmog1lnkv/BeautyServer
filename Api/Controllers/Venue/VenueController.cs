using Api.Controllers.Base;
using Api.Controllers.Venue.Models;
using Api.Filters;
using Api.Utils;
using Application.Features.Services.Queries.GetServicesByVenueId;
using Application.Features.Staffs.Queries.GetStaffWithServicesByVenueId;
using Application.Features.Venues.Commands.AddVenuePhoto;
using Application.Features.Venues.Commands.CreateVenue;
using Application.Features.Venues.Commands.RemoveVenuePhoto;
using Application.Features.Venues.Commands.UpdateVenue;
using Application.Features.Venues.Queries.GetAllVenues;
using Application.Features.Venues.Queries.GetVenueById;
using Application.Features.Venues.Queries.GetVenueClustersInBounds;
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
        if (!HttpContext.IsManager() && !HttpContext.IsAdmin())
            return Unauthorized();
        
        request.OrganizationId = HttpContext.GetStaffOrganizationId();

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
        [FromQuery] string? search,
        [FromQuery] int limit,
        [FromQuery] int offset)
    {
        if (limit <= 0 || offset < 0)
            return BadRequest("Limit must be greater than zero, and offset cannot be negative.");

        var query = new GetAllVenuesQuery(limit, offset, latitude, longitude, search);

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

    [HttpPost("photo")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [StaffValidationFilter]
    public async Task<IActionResult> AddPhoto([FromBody] AddVenuePhotoDto request)
    {
        if (!HttpContext.IsManager())
            return Unauthorized();

        request.StaffId = HttpContext.GetStaffId();

        var command = mapper.Map<AddVenuePhotoCommand>(request);
        var result = await Sender.Send(command);

        return result.IsFailure
            ? HandleFailure(result)
            : Ok();
    }

    [HttpGet("clusters")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [UserValidationFilter]
    public async Task<IActionResult> GetClustersInBounds(
        [FromQuery] double minLatitude,
        [FromQuery] double minLongitude,
        [FromQuery] double maxLatitude,
        [FromQuery] double maxLongitude,
        [FromQuery] int zoom)
    {
        var query = new GetVenueClustersInBoundsQuery(
            minLatitude,
            minLongitude,
            maxLatitude,
            maxLongitude,
            zoom);

        var result = await Sender.Send(query);

        if (result.IsFailure)
            return HandleFailure(result);

        var response = new
        {
            result.Value.Clusters,
            Venues = mapper.Map<List<VenueLookupDto>>(result.Value.Venues)
        };

        return Ok(response);
    }

    [HttpDelete("{id}/photo/{photoId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [StaffValidationFilter]
    public async Task<IActionResult> RemovePhoto(Guid id, Guid photoId)
    {
        if (!HttpContext.IsManager() && !HttpContext.IsAdmin())
            return Unauthorized();

        var command = new RemoveVenuePhotoCommand(HttpContext.GetStaffId(), id, photoId);
        var result = await Sender.Send(command);

        return result.IsFailure
            ? HandleFailure(result)
            : Ok();
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

    [HttpGet("{id}/staff")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStaffWithServicesByVenueId(Guid id)
    {
        var query = new GetStaffWithServicesByVenueIdQuery(id);
        var result = await Sender.Send(query);

        if (result.IsFailure)
            return HandleFailure(result);

        var staffDtos = mapper.Map<List<VenueStaffWithServicesLookupDto>>(result.Value);
        return Ok(staffDtos);
    }

    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [StaffValidationFilter]
    public async Task<IActionResult> Update([FromBody] UpdateVenueDto request)
    {
        if (!HttpContext.IsManager() && !HttpContext.IsAdmin())
            return Unauthorized();

        request.InitiatorId = HttpContext.GetStaffId();
        var command = mapper.Map<UpdateVenueCommand>(request);
        var result = await Sender.Send(command);

        return result.IsFailure
            ? HandleFailure(result)
            : Ok();
    }
}
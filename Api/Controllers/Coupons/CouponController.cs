using Api.Controllers.Base;
using Api.Controllers.Coupons.Models;
using Api.Filters;
using Api.Utils;
using Application.Features.Coupons.Commands.CreateCoupon;
using Application.Features.Coupons.Quries.GetAllPublicCoupons;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Coupons;

[Route("api/coupon")]
public class CouponController(IMapper mapper) : BaseController
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [StaffValidationFilter]
    public async Task<IActionResult> Create([FromBody] CreateCouponDto request)
    {
        if (!HttpContext.IsManager() && !HttpContext.IsAdmin())
            return Unauthorized();

        if (request.OrganizationId == Guid.Empty)
            request.OrganizationId = HttpContext.GetStaffOrganizationId();

        request.InitiatorId = HttpContext.GetStaffId();

        var command = mapper.Map<CreateCouponCommand>(request);
        var result = await Sender.Send(command);

        return result.IsFailure
            ? HandleFailure(result)
            : NoContent();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [UserValidationFilter]
    public async Task<IActionResult> GetAllPublic(
        [FromQuery] string? search,
        [FromQuery] int page = 0,
        [FromQuery] int pageSize = 10)
    {
        HttpContext.GetUserId();
    
        var query = new GetAllPublicCouponsQuery(search, page, pageSize);
        var result = await Sender.Send(query);
    
        if (result.IsFailure)
            return HandleFailure(result);
    
        return Ok(result.Value);
    }
}
using Application.Features.Dto;
using Application.Features.Organizations.Queries.GetOrganizationStaffsById.Dto;
using Application.Messaging.Query;
using Domain.Shared;

namespace Application.Features.Organizations.Queries.GetOrganizationStaffsById;

public record GetOrganizationStaffsByIdQuery(
    Guid Id,
    string? Search,
    int Page = 0,
    int PageSize = 10) : IQuery<Result<PaginatedVm<StaffLookupDto>>>;
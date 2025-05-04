using Application.Features.Dto;
using Application.Features.Organizations.Queries.GetOrganizationServicesById.Dto;
using Application.Messaging.Query;
using Domain.Shared;

namespace Application.Features.Organizations.Queries.GetOrganizationServicesById;

public record GetOrganizationServicesByIdQuery(
    Guid Id,
    string? Search,
    int Page = 0,
    int PageSize = 10) : IQuery<Result<PaginatedVm<ServiceLookupDto>>>;
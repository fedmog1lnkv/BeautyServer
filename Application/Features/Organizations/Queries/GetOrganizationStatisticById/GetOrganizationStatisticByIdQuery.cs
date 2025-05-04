using Application.Features.Organizations.Queries.GetOrganizationStatisticById.Dto;
using Application.Messaging.Query;
using Domain.Entities;
using Domain.Shared;

namespace Application.Features.Organizations.Queries.GetOrganizationStatisticById;

public record GetOrganizationStatisticByIdQuery(Guid Id) : IQuery<Result<OrganizationStatsVm>>;
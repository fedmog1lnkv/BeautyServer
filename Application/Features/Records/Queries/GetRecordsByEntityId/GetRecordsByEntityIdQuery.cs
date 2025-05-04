using Application.Features.Dto;
using Application.Messaging.Query;
using Domain.Entities;
using Domain.Enums;
using Domain.Shared;

namespace Application.Features.Records.Queries.GetRecordsByEntityId;

public record GetRecordsByEntityIdQuery(
    EntityType EntityType,
    Guid EntityId,
    int Page = 0,
    int PageSize = 10) : IQuery<Result<PaginatedVm<Record>>>;
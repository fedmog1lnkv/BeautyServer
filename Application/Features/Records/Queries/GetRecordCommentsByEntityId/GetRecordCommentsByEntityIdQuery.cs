using Application.Features.Dto;
using Application.Features.Records.Queries.GetRecordCommentsByEntityId.Dto;
using Application.Messaging.Query;
using Domain.Enums;
using Domain.Shared;

namespace Application.Features.Records.Queries.GetRecordCommentsByEntityId;

public record GetRecordCommentsByEntityIdQuery(
    EntityType EntityType,
    Guid EntityId,
    int Page = 0,
    int PageSize = 10) : IQuery<Result<PaginatedVm<RecordReviewVm>>>;
using Application.Features.Dto;
using Application.Features.Records.Queries.GetRecordCommentsByEntityId.Dto;
using Application.Messaging.Query;
using Domain.Repositories.Records;
using Domain.Shared;

namespace Application.Features.Records.Queries.GetRecordCommentsByEntityId;

public sealed class GetRecordCommentsByEntityIdQueryHandler(IRecordReadOnlyRepository recordReadOnlyRepository) :
    IQueryHandler<GetRecordCommentsByEntityIdQuery,
        Result<PaginatedVm<RecordReviewVm>>>
{
    public async Task<Result<PaginatedVm<RecordReviewVm>>> Handle(
        GetRecordCommentsByEntityIdQuery request,
        CancellationToken cancellationToken)
    {
        var records = await recordReadOnlyRepository.GetRecordsByEntityIdWithReviews(
            request.EntityType,
            request.EntityId,
            request.PageSize,
            (request.Page) * request.PageSize,
            cancellationToken);

        int totalCount = await recordReadOnlyRepository.GetTotalCountByEntityIdWithReviews(
            request.EntityType,
            request.EntityId,
            cancellationToken);

        var recordReviewVms = records.Select(
            record => new RecordReviewVm
            {
                RecordId = record.Id,
                Rating = record.Review!.Rating,
                Comment = record.Review.Comment
            }).ToList();

        var paginatedResult = new PaginatedVm<RecordReviewVm>
        {
            Data = recordReviewVms,
            TotalCount = totalCount,
            PageNumber = request.Page,
            PageSize = request.PageSize
        };

        return Result.Success(paginatedResult);
    }
}
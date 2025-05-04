using Application.Features.Dto;
using Application.Messaging.Query;
using Domain.Entities;
using Domain.Repositories.Records;
using Domain.Shared;

namespace Application.Features.Records.Queries.GetRecordsByEntityId;

public sealed class GetRecordsByEntityIdQueryHandler(IRecordReadOnlyRepository recordReadOnlyRepository) : 
    IQueryHandler<GetRecordsByEntityIdQuery, 
        Result<PaginatedVm<Record>>>
{
    public async Task<Result<PaginatedVm<Record>>> Handle(GetRecordsByEntityIdQuery request, CancellationToken cancellationToken)
    {
        var records = await recordReadOnlyRepository.GetRecordsByEntityId(
            request.EntityType, 
            request.EntityId, 
            request.PageSize, 
            (request.Page) * request.PageSize, 
            cancellationToken
        );

        int totalCount = await recordReadOnlyRepository.GetTotalCountByEntityId(
            request.EntityType, 
            request.EntityId, 
            cancellationToken
        );

        var paginatedResult = new PaginatedVm<Record>
        {
            Data = records.ToList(),
            TotalCount = totalCount,
            PageNumber = request.Page,
            PageSize = request.PageSize
        };

        return Result.Success(paginatedResult);
    }
}
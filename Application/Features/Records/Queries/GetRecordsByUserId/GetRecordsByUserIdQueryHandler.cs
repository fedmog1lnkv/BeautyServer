using Application.Features.Records.Queries.GetRecordsByStaffId;
using Application.Messaging.Query;
using Domain.Entities;
using Domain.Repositories.Records;
using Domain.Shared;

namespace Application.Features.Records.Queries.GetRecordsByUserId;

public class GetRecordsByUserIdQueryHandler(IRecordReadOnlyRepository repository)
    : IQueryHandler<GetRecordsByUserIdQuery,
        Result<List<Record>>>
{
    public async Task<Result<List<Record>>> Handle(
        GetRecordsByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        var records =
            await repository.GetByUserIdAsync(
                request.UserId,
                request.Limit,
                request.Offset,
                request.IsPending,
                cancellationToken);

        return Result.Success(records);
    }
}
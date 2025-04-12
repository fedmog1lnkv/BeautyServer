using Application.Messaging.Query;
using Domain.Entities;
using Domain.Repositories.Records;
using Domain.Shared;

namespace Application.Features.Records.Queries.GetApprovedRecords;

public class GetRecordsByUserIdQueryHandler(IRecordReadOnlyRepository repository)
    : IQueryHandler<GetApprovedRecordsQuery,
        Result<List<Record>>>
{
    public async Task<Result<List<Record>>> Handle(
        GetApprovedRecordsQuery request,
        CancellationToken cancellationToken)
    {
        var records = await repository.GetApprovedRecordsFromTime(DateTime.UtcNow, cancellationToken);

        return Result.Success(records);
    }
}
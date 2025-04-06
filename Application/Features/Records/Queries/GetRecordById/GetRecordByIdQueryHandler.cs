using Application.Messaging.Query;
using Domain.Entities;
using Domain.Errors;
using Domain.Repositories.Records;
using Domain.Shared;

namespace Application.Features.Records.Queries.GetRecordById;

public class GetRecordByIdQueryHandler(IRecordReadOnlyRepository repository)
    : IQueryHandler<GetRecordByIdQuery,
        Result<Record>>
{
    public async Task<Result<Record>> Handle(
        GetRecordByIdQuery request,
        CancellationToken cancellationToken)
    {
        var record =
            await repository.GetRecordById(
                request.Id,
                cancellationToken);

        if (record is null)
            return Result.Failure<Record>(DomainErrors.Record.NotFound);

        return Result.Success(record);
    }
}
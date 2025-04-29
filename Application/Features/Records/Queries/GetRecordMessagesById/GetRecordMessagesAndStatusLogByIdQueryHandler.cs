using Application.Messaging.Query;
using Domain.Entities;
using Domain.Errors;
using Domain.Repositories.Records;
using Domain.Shared;

namespace Application.Features.Records.Queries.GetRecordMessagesById;

public class GetRecordMessagesAndStatusLogByIdQueryHandler(IRecordReadOnlyRepository repository)
    : IQueryHandler<GetRecordMessagesAndStatusLogByIdQuery,
        Result<Record>>
{
    public async Task<Result<Record>> Handle(
        GetRecordMessagesAndStatusLogByIdQuery request,
        CancellationToken cancellationToken)
    {
        var record =
            await repository.GetByIdWithMessagesAndStatusLog(
                request.Id,
                cancellationToken);


        if (record is null)
            return Result.Failure<Record>(DomainErrors.Record.NotFound);

        if (request.InitiatorId != record.StaffId && request.InitiatorId != record.UserId)
            return Result.Failure<Record>(DomainErrors.RecordChat.NoAccess(request.InitiatorId));

        return Result.Success(record);
    }
}
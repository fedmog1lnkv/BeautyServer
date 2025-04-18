using Application.Features.Services.Queries.GetServicesByVenueId;
using Application.Messaging.Query;
using Domain.Entities;
using Domain.Errors;
using Domain.Repositories.Records;
using Domain.Shared;

namespace Application.Features.Records.Queries.GetRecordsByStaffId;

public class GetRecordsByStaffIdQueryHandler(IRecordReadOnlyRepository repository)
    : IQueryHandler<GetRecordsByStaffIdQuery,
        Result<List<Record>>>
{
    public async Task<Result<List<Record>>> Handle(
        GetRecordsByStaffIdQuery request,
        CancellationToken cancellationToken)
    {
        var records =
            await repository.GetByStaffIdAndDateAsync(
                request.StaffId,
                request.Date,
                request.Limit,
                request.Offset,
                request.IsPending,
                cancellationToken);

        return Result.Success(records);
    }
}
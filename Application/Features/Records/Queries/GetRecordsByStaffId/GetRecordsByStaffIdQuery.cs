using Application.Messaging.Query;
using Domain.Entities;
using Domain.Shared;

namespace Application.Features.Records.Queries.GetRecordsByStaffId;

public record GetRecordsByStaffIdQuery(Guid StaffId, int Limit, int Offset, bool IsPending) : IQuery<Result<List<Record>>>;
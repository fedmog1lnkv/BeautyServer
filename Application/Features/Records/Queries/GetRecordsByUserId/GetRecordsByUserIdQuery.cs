using Application.Messaging.Query;
using Domain.Entities;
using Domain.Shared;

namespace Application.Features.Records.Queries.GetRecordsByUserId;

public record GetRecordsByUserIdQuery(Guid UserId, int Limit, int Offset, bool IsPending) : IQuery<Result<List<Record>>>;
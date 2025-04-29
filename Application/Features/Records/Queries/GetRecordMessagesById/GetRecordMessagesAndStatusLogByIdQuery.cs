using Application.Messaging.Query;
using Domain.Entities;
using Domain.Shared;

namespace Application.Features.Records.Queries.GetRecordMessagesById;

public record GetRecordMessagesAndStatusLogByIdQuery(Guid Id, Guid InitiatorId) : IQuery<Result<Record>>;
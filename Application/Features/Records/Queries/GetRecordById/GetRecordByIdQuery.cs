using Application.Messaging.Query;
using Domain.Entities;
using Domain.Shared;

namespace Application.Features.Records.Queries.GetRecordById;

public record GetRecordByIdQuery(Guid Id) : IQuery<Result<Record>>;
using Application.Messaging.Query;
using Domain.Entities;
using Domain.Shared;

namespace Application.Features.Records.Queries.GetApprovedRecords;

public record GetApprovedRecordsQuery : IQuery<Result<List<Record>>>;
using Application.Features.Organizations.Queries.GetOrganizationStatisticById.Dto;
using Application.Messaging.Query;
using Domain.Enums;
using Domain.Repositories.Records;
using Domain.Shared;

namespace Application.Features.Organizations.Queries.GetOrganizationStatisticById;

public class GetOrganizationStatisticByIdQueryHandler(IRecordReadOnlyRepository recordReadOnlyRepository)
    : IQueryHandler<GetOrganizationStatisticByIdQuery,
        Result<OrganizationStatsVm>>
{
    public async Task<Result<OrganizationStatsVm>> Handle(
        GetOrganizationStatisticByIdQuery request,
        CancellationToken cancellationToken)
    {
        var records = await recordReadOnlyRepository.GetByOrganizationIdAsync(
            request.Id,
            cancellationToken);

        var response = new OrganizationStatsVm
        {
            CompletedRecordsCount = records.Count(r => r.Status == RecordStatus.Completed),
            UniqueCustomersCount = records.Select(r => r.UserId).Distinct().Count(),
            AverageRating = (int)records
                .Where(r => r.Review?.Rating is not null)
                .Select(r => r.Review!.Rating)
                .DefaultIfEmpty(0)
                .Average(),
            TotalEarned = (int)records.Where(r => r.Status == RecordStatus.Completed)
                .Sum(r => (r.Service.Price?.Value ?? 0))
        };

        return response;
    }
}
using Application.Messaging.Query;
using Domain.Entities;
using Domain.Repositories.Staffs;
using Domain.Shared;

namespace Application.Features.Staffs.Queries.GetStaffWithServicesByVenueId;

public class GetStaffWithServicesByVenueIdQueryHandler(IStaffReadOnlyRepository staffRepository)
    : IQueryHandler<GetStaffWithServicesByVenueIdQuery,
        Result<List<Staff>>>
{
    public async Task<Result<List<Staff>>> Handle(
        GetStaffWithServicesByVenueIdQuery request,
        CancellationToken cancellationToken)
    {
        var staff = await staffRepository.GetByVenueIdWithServicesAsync(
            request.VenueId,
            cancellationToken);

        return Result.Success(staff);
    }
}
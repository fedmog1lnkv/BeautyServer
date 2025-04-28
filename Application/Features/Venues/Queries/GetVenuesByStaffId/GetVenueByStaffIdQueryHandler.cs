using Application.Messaging.Query;
using Domain.Entities;
using Domain.Errors;
using Domain.Repositories.Staffs;
using Domain.Repositories.Venues;
using Domain.Shared;

namespace Application.Features.Venues.Queries.GetVenuesByStaffId;

public class GetVenuesByStaffIdQueryHandler(
    IVenueReadOnlyRepository repository,
    IStaffReadOnlyRepository staffReadOnlyRepository) :
    IQueryHandler<GetVenuesByStaffIdQuery,
        Result<List<Venue>>>
{
    public async Task<Result<List<Venue>>> Handle(GetVenuesByStaffIdQuery request, CancellationToken cancellationToken)
    {
        var staff = await staffReadOnlyRepository.GetByIdAsync(request.StaffId, cancellationToken);
        if (staff is null)
            return Result.Failure<List<Venue>>(DomainErrors.Staff.NotFound(request.StaffId));

        var venues = await repository.GetByOrganizationId(
            staff.OrganizationId,
            cancellationToken);

        return Result.Success(venues);
    }
}
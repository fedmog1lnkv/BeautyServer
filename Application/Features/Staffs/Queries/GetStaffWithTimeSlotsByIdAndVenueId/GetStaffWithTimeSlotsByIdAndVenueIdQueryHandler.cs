using Application.Messaging.Query;
using Domain.Entities;
using Domain.Errors;
using Domain.Repositories.Staffs;
using Domain.Shared;

namespace Application.Features.Staffs.Queries.GetStaffWithTimeSlotsByIdAndVenueId;

public class GetStaffWithTimeSlotsByIdAndVenueIdQueryHandler(IStaffReadOnlyRepository staffRepository)
    : IQueryHandler<GetStaffWithTimeSlotsByIdAndVenueIdQuery,
        Result<Staff>>
{
    public async Task<Result<Staff>> Handle(
        GetStaffWithTimeSlotsByIdAndVenueIdQuery request,
        CancellationToken cancellationToken)
    {
        var staff = await staffRepository.GetByIdAndVenueIdWithTimeSlots(
            request.StaffId,
            request.VenueId,
            cancellationToken);

        if (staff is null)
            return Result.Failure<Staff>(DomainErrors.Staff.NotFound(request.StaffId));

        return Result.Success(staff);
    }
}
using Application.Messaging.Query;
using Domain.Entities;
using Domain.Errors;
using Domain.Repositories.Staffs;
using Domain.Shared;

namespace Application.Features.Staffs.Queries.GetStaffWithServicesById;

public class GetStaffWithServicesByIdQueryHandler(IStaffReadOnlyRepository repository)
    : IQueryHandler<GetStaffWithServicesByIdQuery,
        Result<Staff>>
{
    public async Task<Result<Staff>> Handle(GetStaffWithServicesByIdQuery request, CancellationToken cancellationToken)
    {
        var staff = await repository.GetByIdWithServices(
            request.Id,
            cancellationToken);

        if (staff is null)
            return Result.Failure<Staff>(DomainErrors.Staff.NotFound(request.Id));

        return staff;
    }
}
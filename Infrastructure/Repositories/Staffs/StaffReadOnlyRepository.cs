using Domain.Repositories.Staffs;
using Domain.ValueObjects;

namespace Infrastructure.Repositories.Staffs;

public class StaffReadOnlyRepository : IStaffReadOnlyRepository
{
    public async Task<bool> IsPhoneNumberUnique(
        StaffPhoneNumber phoneNumber,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
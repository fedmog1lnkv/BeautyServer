using Domain.Entities;
using Domain.Repositories.Base;
using Domain.ValueObjects;

namespace Domain.Repositories.Staffs;

public interface IStaffReadOnlyRepository : IReadOnlyRepository<Staff>
{
    Task<bool> IsPhoneNumberUnique(StaffPhoneNumber phoneNumber, CancellationToken cancellationToken = default);
    Task<Staff?> GetByIdWithServices(Guid id, CancellationToken cancellationToken = default);
}
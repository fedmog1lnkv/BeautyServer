using Domain.Entities;
using Domain.Repositories.Base;
using Domain.ValueObjects;

namespace Domain.Repositories.Staffs;

public interface IStaffRepository : IRepository<Staff>
{
    Task<Staff?> GetByIdWithServices(Guid id, CancellationToken cancellationToken = default);
    Task<Staff?> GetByPhoneNumberAsync(StaffPhoneNumber phoneNumber, CancellationToken cancellationToken = default);
    void Add(Staff staff);
}
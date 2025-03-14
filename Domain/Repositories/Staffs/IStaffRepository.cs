using Domain.Entities;
using Domain.Repositories.Base;

namespace Domain.Repositories.Staffs;

public interface IStaffRepository : IRepository<Staff>
{
    Task<Staff?> GetByIdWithServices(Guid id, CancellationToken cancellationToken = default);
    void Add(Staff staff);
}
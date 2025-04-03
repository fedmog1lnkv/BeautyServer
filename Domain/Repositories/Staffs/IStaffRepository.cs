using Domain.Entities;
using Domain.Repositories.Base;
using Domain.ValueObjects;

namespace Domain.Repositories.Staffs;

public interface IStaffRepository : IRepository<Staff>
{
    Task<Staff?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Staff?> GetByIdWithServicesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Staff?> GetByIdWithTimeSlotsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Staff?> GetByPhoneNumberAsync(StaffPhoneNumber phoneNumber, CancellationToken cancellationToken = default);
    void Add(Staff staff);
    Task<string?> UploadPhotoAsync(string base64Photo, string fileName);
}
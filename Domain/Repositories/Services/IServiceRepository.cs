using Domain.Entities;
using Domain.Repositories.Base;

namespace Domain.Repositories.Services;

public interface IServiceRepository : IRepository<Service>
{
    Task<Service?> GetById(Guid serviceId, CancellationToken cancellationToken = default);
    Task<List<Service>> GetByOrganizationId(Guid organizationId, CancellationToken cancellationToken = default);
    void Add(Service service);
    void Remove(Service service);
    Task<string?> UploadPhotoAsync(string base64Photo, string fileName);
    Task<bool> DeletePhoto(string photoUrl);
}
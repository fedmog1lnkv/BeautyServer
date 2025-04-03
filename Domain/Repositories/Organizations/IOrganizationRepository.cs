using Domain.Entities;
using Domain.Repositories.Base;

namespace Domain.Repositories.Organizations;

public interface IOrganizationRepository : IRepository<Organization>
{
    Task<Organization?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(Organization organization);
    void Remove(Organization organization);
    
    Task<string?> UploadPhotoAsync(string base64Photo, string fileName);
}
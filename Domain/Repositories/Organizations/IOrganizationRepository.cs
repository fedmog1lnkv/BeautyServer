using Domain.Entities;
using Domain.Repositories.Base;

namespace Domain.Repositories.Organizations;

public interface IOrganizationRepository : IRepository<Organization>
{
    void Add(Organization organization);
}
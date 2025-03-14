using Domain.Entities;
using Domain.Repositories.Base;

namespace Domain.Repositories.Services;

public interface IServiceRepository : IRepository<Service>
{
    void Add(Service service);
}
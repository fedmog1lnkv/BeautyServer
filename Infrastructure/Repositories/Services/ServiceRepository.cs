using Domain.Entities;
using Domain.Repositories.Services;

namespace Infrastructure.Repositories.Services;

public class ServiceRepository(ApplicationDbContext dbContext) : IServiceRepository
{
    public void Add(Service service)
    {
        dbContext.Set<Service>().Add(service);
        dbContext.SaveChanges();
    }
}
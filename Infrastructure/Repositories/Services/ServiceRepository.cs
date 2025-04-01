using Domain.Entities;
using Domain.Repositories.Services;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Services;

public class ServiceRepository(ApplicationDbContext dbContext) : IServiceRepository
{
    public async Task<Service?> GetById(Guid serviceId, CancellationToken cancellationToken = default) =>
        await dbContext.Set<Service>()
            .FirstOrDefaultAsync(s => s.Id == serviceId, cancellationToken);

    public void Add(Service service) =>
        dbContext.Set<Service>().Add(service);

    public void Remove(Service service) =>
        dbContext.Set<Service>().Remove(service);
}
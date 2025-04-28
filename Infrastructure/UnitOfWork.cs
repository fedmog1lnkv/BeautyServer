using Domain.Primitives;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public sealed class UnitOfWork(ApplicationDbContext dbContext) : IUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateModifiedOnUtc();
        return dbContext.SaveChangesAsync(cancellationToken);
    }

    public IReadOnlyList<IDomainEvent> GetDomainEvents()
    {
        var domainEntities = dbContext.ChangeTracker.Entries<AggregateRoot>()
            .Where(e => e.Entity.GetDomainEvents().Any())
            .Select(e => e.Entity)
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(e => e.GetDomainEvents())
            .ToList();

        return domainEvents;
    }

    public void ClearDomainEvents()
    {
        var domainEntities = dbContext.ChangeTracker.Entries<AggregateRoot>()
            .Where(e => e.Entity.GetDomainEvents().Any()) // Проверяем, есть ли доменные события
            .Select(e => e.Entity)
            .ToList();

        foreach (var entity in domainEntities)
        {
            entity.ClearDomainEvents();
        }
    }

    private void UpdateModifiedOnUtc()
    {
        var entries = dbContext.ChangeTracker.Entries()
            .Where(e => e is { State: EntityState.Modified, Entity: IAuditableEntity });

        foreach (var entry in entries)
        {
            ((IAuditableEntity)entry.Entity).ModifiedOnUtc = DateTime.UtcNow;
        }
    }
}
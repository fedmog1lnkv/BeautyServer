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
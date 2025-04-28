using Domain.Primitives;

namespace Domain.Repositories;

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    IReadOnlyList<IDomainEvent> GetDomainEvents();
    void ClearDomainEvents();
}
using Domain.Entities;
using Domain.Repositories.Base;

namespace Domain.Repositories.Records;

public interface IRecordReadOnlyRepository : IReadOnlyRepository<Record>
{
    Task<List<Record>> GetByStaffIdAsync(
        Guid staffId,
        int limit,
        int offset,
        bool isPending,
        CancellationToken cancellationToken = default);

    Task<List<Record>> GetByUserIdAsync(
        Guid userId,
        int limit,
        int offset,
        bool isPending,
        CancellationToken cancellationToken = default);

    Task<Record?> GetRecordById(
        Guid id,
        CancellationToken cancellationToken = default);
}
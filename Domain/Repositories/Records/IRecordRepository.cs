using Domain.Entities;
using Domain.Repositories.Base;

namespace Domain.Repositories.Records;

public interface IRecordRepository : IRepository<Record>
{
    Task<Record?> GetRecordById(
        Guid id,
        CancellationToken cancellationToken = default);
    void Add(Record record);
}
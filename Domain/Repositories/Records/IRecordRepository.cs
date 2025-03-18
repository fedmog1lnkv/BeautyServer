using Domain.Entities;
using Domain.Repositories.Base;

namespace Domain.Repositories.Records;

public interface IRecordRepository : IRepository<Record>
{
    void Add(Record record);
}
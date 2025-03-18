using Domain.Entities;
using Domain.Repositories.Records;

namespace Infrastructure.Repositories.Records;

public class RecordRepository(ApplicationDbContext dbContext) : IRecordRepository
{
    public void Add(Record record) =>
        dbContext.Set<Record>().Add(record);
}
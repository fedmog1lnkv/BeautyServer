using Domain.Entities;
using Domain.Repositories.Records;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Records;

public class RecordRepository(ApplicationDbContext dbContext) : IRecordRepository
{
    public async Task<Record?> GetRecordById(
        Guid id,
        CancellationToken cancellationToken = default) =>
        await dbContext.Set<Record>()
            .Where(r => r.Id == id)
            .Include(r => r.User)
            .Include(r => r.Staff)
            .Include(r => r.Service)
            .Include(r => r.Venue)
            .AsSplitQuery()
            .FirstOrDefaultAsync(cancellationToken);

    public void Add(Record record) =>
        dbContext.Set<Record>().Add(record);
}
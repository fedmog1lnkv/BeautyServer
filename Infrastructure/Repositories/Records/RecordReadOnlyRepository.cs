using Domain.Entities;
using Domain.Repositories.Records;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Records;

public class RecordReadOnlyRepository(ApplicationDbContext dbContext) : IRecordReadOnlyRepository
{
    public async Task<List<Record>> GetByStaffIdAsync(
        Guid staffId,
        int limit,
        int offset,
        bool isPending,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<Record>()
            .Where(r => r.StaffId == staffId)
            .Include(r => r.User)
            .Include(r => r.Service)
            .Include(r => r.Venue)
            .Include(r => r.Organization)
            .OrderBy(r => r.StartTimestamp)
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Record>> GetByUserIdAsync(
        Guid userId,
        int limit,
        int offset,
        bool isPending,
        CancellationToken cancellationToken = default) =>
        await dbContext.Set<Record>()
            .Where(r => r.UserId == userId)
            .Include(r => r.Staff)
            .Include(r => r.Service)
            .Include(r => r.Venue)
            .OrderBy(r => r.StartTimestamp)
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);

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
}
using Domain.Entities;
using Domain.Enums;
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
            .Where(
                r => r.StaffId == staffId)
            .Include(r => r.User)
            .Include(r => r.Service)
            .Include(r => r.Venue)
            .Include(r => r.Organization)
            .OrderBy(r => r.StartTimestamp)
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }
}
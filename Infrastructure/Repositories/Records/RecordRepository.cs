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

    public async Task<List<Record>> GetByStaffId(
        Guid staffId,
        int limit,
        int offset,
        CancellationToken cancellationToken = default) =>
        await dbContext.Set<Record>()
            .AsNoTracking()
            .Where(r => r.StaffId == staffId)
            .OrderByDescending(r => r.CreatedOnUtc)
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);

    public async Task<List<Record>> GetByServiceId(
        Guid serviceId,
        int limit,
        int offset,
        CancellationToken cancellationToken = default) =>
        await dbContext.Set<Record>()
            .AsNoTracking()
            .Where(r => r.ServiceId == serviceId)
            .OrderByDescending(r => r.CreatedOnUtc)
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);

    public async Task<List<Record>> GetByVenueId(
        Guid venueId,
        int limit,
        int offset,
        CancellationToken cancellationToken = default) =>
        await dbContext.Set<Record>()
            .AsNoTracking()
            .Where(r => r.VenueId == venueId)
            .OrderByDescending(r => r.CreatedOnUtc)
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);

    public void Add(Record record) =>
        dbContext.Set<Record>().Add(record);
}
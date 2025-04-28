using Domain.Entities;
using Domain.Enums;
using Domain.Repositories.Records;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Records;

public class RecordReadOnlyRepository(ApplicationDbContext dbContext) : IRecordReadOnlyRepository
{
    public async Task<List<Record>> GetByStaffIdAndDateAsync(
        Guid staffId,
        DateOnly date,
        int limit,
        int offset,
        bool isPending,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<Record>()
            .AsNoTracking()
            .Where(r => r.StaffId == staffId && r.StartTimestamp.Date == date.ToDateTime(TimeOnly.MinValue).Date)
            .Include(r => r.User)
            .Include(r => r.Staff)
            .Include(r => r.Service)
            .Include(r => r.Venue)
            .Include(r => r.Organization)
            .OrderBy(r => r.StartTimestamp)
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Record>> GetByStaffIdAndMonth(
        Guid staffId,
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        var startMonth = new DateTimeOffset(new DateTime(year, month, 1), TimeSpan.Zero);
        var endMonth = new DateTimeOffset(new DateTime(year, month, DateTime.DaysInMonth(year, month)), TimeSpan.Zero);

        return await dbContext.Set<Record>()
            .AsNoTracking()
            .Where(
                r => r.StaffId == staffId
                     && r.StartTimestamp.DateTime.Date >= startMonth
                     && r.EndTimestamp.DateTime.Date <= endMonth)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Record>> GetRecordsWithVenueByStaffIdAndDate(
        Guid staffId,
        DateOnly date,
        CancellationToken cancellationToken = default) =>
        await dbContext.Set<Record>()
            .AsNoTracking()
            .Where(
                r => r.StaffId == staffId &&
                     r.StartTimestamp.Date == date.ToDateTime(TimeOnly.MinValue).Date)
            .Include(r => r.Venue)
            .ToListAsync(cancellationToken: cancellationToken);

    public async Task<List<Record>> GetByUserIdAsync(
        Guid userId,
        int limit,
        int offset,
        bool isPending,
        CancellationToken cancellationToken = default) =>
        await dbContext.Set<Record>()
            .AsNoTracking()
            .Where(r => r.UserId == userId)
            .Include(r => r.Staff)
            .Include(r => r.Service)
            .Include(r => r.Venue)
            .OrderByDescending(r => r.CreatedOnUtc)
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);

    public async Task<Record?> GetRecordById(
        Guid id,
        CancellationToken cancellationToken = default) =>
        await dbContext.Set<Record>()
            .AsNoTracking()
            .Where(r => r.Id == id)
            .Include(r => r.User)
            .Include(r => r.Staff)
            .Include(r => r.Service)
            .Include(r => r.Venue)
            .AsSplitQuery()
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<Record?> GetByIdWithMessages(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.Set<Record>()
            .AsNoTracking()
            .Where(r => r.Id == id)
            .Include(r => r.Messages)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<List<Record>> GetApprovedRecordsFromTime(
        DateTime dateTime,
        CancellationToken cancellationToken = default) =>
        await dbContext.Set<Record>()
            .AsNoTracking()
            .Where(r => r.Status == RecordStatus.Approved && r.StartTimestamp >= dateTime)
            .Include(r => r.User)
            .Include(r => r.Venue)
            .ToListAsync(cancellationToken: cancellationToken);
}
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
            .Include(r => r.Messages)
            .OrderBy(r => r.StartTimestamp)
            .Skip(offset)
            .Take(limit)
            .AsSplitQuery()
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

    public async Task<List<Record>> GetRecordsByEntityId(
        EntityType entityType,
        Guid entityId,
        int limit,
        int offset,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Set<Record>().AsNoTracking();

        query = entityType switch
        {
            EntityType.User => query.Where(r => r.UserId == entityId),
            EntityType.Staff => query.Where(r => r.StaffId == entityId),
            EntityType.Service => query.Where(r => r.ServiceId == entityId),
            EntityType.Venue => query.Where(r => r.VenueId == entityId),
            _ => throw new ArgumentOutOfRangeException(nameof(entityType), entityType, null)
        };

        query = query.Include(r => r.User)
            .Include(r => r.Staff)
            .Include(r => r.Service)
            .Include(r => r.Venue)
            .Include(r => r.Messages)
            .OrderBy(r => r.StartTimestamp)
            .Skip(offset)
            .Take(limit)
            .AsSplitQuery();

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<int> GetTotalCountByEntityIdWithReviews(
        EntityType entityType,
        Guid entityId,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Record> query = dbContext.Set<Record>()
            .AsNoTracking()
            .Where(r => r.Review != null);

        query = entityType switch
        {
            EntityType.User => query.Where(r => r.UserId == entityId),
            EntityType.Staff => query.Where(r => r.StaffId == entityId),
            EntityType.Service => query.Where(r => r.ServiceId == entityId),
            EntityType.Venue => query.Where(r => r.VenueId == entityId),
            _ => throw new ArgumentOutOfRangeException(nameof(entityType), entityType, null)
        };

        return await query.CountAsync(cancellationToken);
    }

    public async Task<List<Record>> GetRecordsByEntityIdWithReviews(
        EntityType entityType,
        Guid entityId,
        int limit,
        int offset,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Set<Record>()
            .AsNoTracking()
            .Where(r => r.Review != null)
            .AsQueryable();

        query = entityType switch
        {
            EntityType.User => query.Where(r => r.UserId == entityId),
            EntityType.Staff => query.Where(r => r.StaffId == entityId),
            EntityType.Service => query.Where(r => r.ServiceId == entityId),
            EntityType.Venue => query.Where(r => r.VenueId == entityId),
            _ => throw new ArgumentOutOfRangeException(nameof(entityType), entityType, null)
        };

        return await query
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetTotalCountByEntityId(
        EntityType entityType,
        Guid entityId,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Record> query = dbContext.Set<Record>().AsNoTracking();

        query = entityType switch
        {
            EntityType.User => query.Where(r => r.UserId == entityId),
            EntityType.Staff => query.Where(r => r.StaffId == entityId),
            EntityType.Service => query.Where(r => r.ServiceId == entityId),
            EntityType.Venue => query.Where(r => r.VenueId == entityId),
            _ => throw new ArgumentOutOfRangeException(nameof(entityType), entityType, null)
        };

        return await query.CountAsync(cancellationToken);
    }

    public async Task<List<Record>> GetByUserId(
        Guid userId,
        int limit,
        int offset,
        CancellationToken cancellationToken = default) =>
        await dbContext.Set<Record>()
            .AsNoTracking()
            .Where(r => r.UserId == userId)
            .Include(r => r.Staff)
            .Include(r => r.Service)
            .Include(r => r.Venue)
            .Include(r => r.Messages)
            .OrderByDescending(r => r.CreatedOnUtc)
            .Skip(offset)
            .Take(limit)
            .AsSplitQuery()
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
            .Include(r => r.Messages)
            .AsSplitQuery()
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<Record?> GetByIdWithMessagesAndStatusLog(
        Guid id,
        CancellationToken cancellationToken = default) =>
        await dbContext.Set<Record>()
            .AsNoTracking()
            .Where(r => r.Id == id)
            .Include(r => r.Messages)
            .Include(r => r.StatusLogs)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<List<Record>> GetByOrganizationIdAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default) =>
        await dbContext.Set<Record>()
            .AsNoTracking()
            .Where(r => r.OrganizationId == organizationId)
            .Include(r => r.Service)
            .ToListAsync(cancellationToken);

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
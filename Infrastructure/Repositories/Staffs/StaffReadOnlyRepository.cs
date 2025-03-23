using Domain.Entities;
using Domain.Repositories.Staffs;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Staffs;

public class StaffReadOnlyRepository(ApplicationDbContext dbContext) : IStaffReadOnlyRepository
{
    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.Set<Staff>()
            .AsNoTracking()
            .AnyAsync(s => s.Id == id, cancellationToken);

    public async Task<bool> IsPhoneNumberUnique(
        StaffPhoneNumber phoneNumber,
        CancellationToken cancellationToken = default)
    {
        var staffExists = await dbContext.Set<Staff>()
            .AsNoTracking()
            .AnyAsync(s => s.PhoneNumber == phoneNumber, cancellationToken);

        return !staffExists;
    }

    public async Task<Staff?> GetByIdWithServices(
        Guid id,
        CancellationToken cancellationToken = default) =>
        await dbContext.Set<Staff>()
            .AsNoTracking()
            .Include(s => s.Services)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<Staff?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.Set<Staff>()
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<List<Staff>> GetByVenueIdWithServicesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        return await dbContext.Set<Staff>()
            .AsNoTracking()
            .Where(s => s.TimeSlots.Any(ts => ts.VenueId == id && ts.Date >= today))
            .Include(s => s.Services)
            .ToListAsync(cancellationToken: cancellationToken);
    }
}
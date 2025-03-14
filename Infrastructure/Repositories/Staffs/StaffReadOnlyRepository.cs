using Domain.Entities;
using Domain.Repositories.Staffs;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Staffs;

public class StaffReadOnlyRepository(ApplicationDbContext dbContext) : IStaffReadOnlyRepository
{
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
}
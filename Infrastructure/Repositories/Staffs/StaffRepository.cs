using Domain.Entities;
using Domain.Repositories.Staffs;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Staffs;

public class StaffRepository(ApplicationDbContext dbContext) : IStaffRepository
{
    public async Task<Staff?> GetByIdWithServices(
        Guid id,
        CancellationToken cancellationToken = default) =>
        await dbContext.Set<Staff>()
            .Include(s => s.Services)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<Staff?> GetByPhoneNumberAsync(
        StaffPhoneNumber phoneNumber,
        CancellationToken cancellationToken = default) =>
        await dbContext.Set<Staff>()
            .FirstOrDefaultAsync(s => s.PhoneNumber == phoneNumber, cancellationToken);

    public void Add(Staff staff)
    {
        dbContext.Set<Staff>().Add(staff);
        dbContext.SaveChanges();
    }
}
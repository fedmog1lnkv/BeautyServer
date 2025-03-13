using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);
        base.OnModelCreating(builder);

        builder.HasPostgresEnum<OrganizationSubscription>();
        builder.Entity<Organization>()
            .Property(o => o.Subscription)
            .HasColumnType("organization_subscription");
    }
}
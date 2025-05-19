using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityTypeConfigurations;

public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
    {
        builder.ToTable(TableNames.Coupons);

        builder.HasKey(o => o.Id);

        builder.Property(c => c.Name)
            .HasConversion(
                name => name.Value,
                value => CouponName.Create(value).Value)
            .HasMaxLength(CouponName.MaxLength)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasConversion(
                description => description.Value,
                value => CouponDescription.Create(value).Value)
            .HasMaxLength(CouponDescription.MaxLength)
            .IsRequired(false);

        builder.Property(c => c.Code)
            .HasConversion(
                code => code.Value,
                value => CouponCode.Create(value).Value)
            .HasMaxLength(CouponCode.Length)
            .IsRequired();

        builder.Property(c => c.DiscountType)
            .HasConversion(
                v => v.ToString(),
                v => (CouponDiscountType)Enum.Parse(typeof(CouponDiscountType), v, true))
            .HasColumnType("varchar(10)")
            .IsRequired();

        builder.Property(c => c.DiscountValue)
            .HasConversion(
                dv => dv.Value,
                value => CouponDiscountValue.Create(value).Value)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(c => c.IsPublic)
            .IsRequired();

        builder.OwnsOne(
            c => c.UsageLimit,
            ul =>
            {
                ul.Property(u => u.Total)
                    .HasColumnName("UsageLimitTotal")
                    .IsRequired();

                ul.Property(u => u.Remaining)
                    .HasColumnName("UsageLimitRemaining")
                    .IsRequired();
            });

        builder.Property(c => c.StartDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(c => c.EndDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(c => c.CreatedOnUtc)
            .IsRequired();

        builder.Property(c => c.ModifiedOnUtc)
            .IsRequired(false);
        
        builder.HasOne(c => c.Organization)
            .WithMany(o => o.Coupons)
            .HasForeignKey(c => c.OrganizationId);
    }
}
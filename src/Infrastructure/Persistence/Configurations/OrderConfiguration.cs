using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> e)
    {
        e.HasKey(x => x.Id);
        e.Property(x => x.TotalAmount)
            .HasConversion(v => v.Value, v => new Domain.ValueObjects.Money(v))
            .HasColumnType("decimal(18,2)");
        e.Property(x => x.OrderDate).IsRequired();
        e.Property(x => x.Status).HasConversion<int>().IsRequired();
        e.HasOne(x => x.Customer).WithMany(c => c.Orders).HasForeignKey(x => x.CustomerId);
    }
}

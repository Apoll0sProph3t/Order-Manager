using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> e)
    {
        e.HasKey(x => x.Id);
        e.Property(x => x.UnitPrice)
            .HasConversion(v => v.Value, v => new Domain.ValueObjects.Money(v))
            .HasColumnType("decimal(18,2)");
        e.Property(x => x.TotalPrice)
            .HasConversion(v => v.Value, v => new Domain.ValueObjects.Money(v))
            .HasColumnType("decimal(18,2)");
        e.Property(x => x.ProductName).HasMaxLength(200).IsRequired();
        e.HasOne(x => x.Order).WithMany(o => o.Items).HasForeignKey(x => x.OrderId);
        e.HasOne(x => x.Product).WithMany(p => p.OrderItems).HasForeignKey(x => x.ProductId);
    }
}

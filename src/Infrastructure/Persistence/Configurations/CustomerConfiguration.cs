using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> e)
    {
        e.HasKey(x => x.Id);
        e.Property(x => x.Name).HasMaxLength(200).IsRequired();
        e.Property(x => x.Email)
            .HasConversion(v => v.Value, v => new Domain.ValueObjects.Email(v))
            .HasMaxLength(200).IsRequired();
        e.Property(x => x.Phone)
            .HasConversion(v => v.Value, v => new Domain.ValueObjects.Phone(v))
            .HasMaxLength(50).IsRequired();
    }
}

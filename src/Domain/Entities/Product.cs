using Domain.Entities.BaseEntity;
using Domain.ValueObjects;

namespace Domain.Entities;

public class Product : Entity
{
    public string Name { get; set; } = null!;
    public Money Price { get; set; }
    public List<OrderItem> OrderItems { get; set; } = new();
}

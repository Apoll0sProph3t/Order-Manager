using Domain.Entities.BaseEntity;

namespace Domain.Entities;

public class Product : Entity
{
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public List<OrderItem> OrderItems { get; set; } = new();
}

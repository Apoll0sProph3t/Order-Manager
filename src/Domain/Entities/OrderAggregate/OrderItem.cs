using Domain.Entities.BaseEntity;
using Domain.ValueObjects;

namespace Domain.Entities;

public class OrderItem : Entity
{
    public Guid OrderId { get; set; }
    public Order? Order { get; set; }
    public Guid ProductId { get; set; }
    public Product? Product { get; set; }
    public string ProductName { get; set; } = null!;
    public int Quantity { get; set; }
    public Money UnitPrice { get; set; }
    public Money TotalPrice { get; set; }
}

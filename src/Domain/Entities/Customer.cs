using Domain.Entities.BaseEntity;
using Domain.ValueObjects;

namespace Domain.Entities;

public class Customer : Entity
{
    public string Name { get; set; } = null!;
    public Email Email { get; set; } = null!;
    public Phone Phone { get; set; } = null!;
    public List<Order> Orders { get; set; } = new();
}

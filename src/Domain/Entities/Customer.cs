using Domain.Entities.BaseEntity;

namespace Domain.Entities;

public class Customer : Entity
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public List<Order> Orders { get; set; } = new();
}

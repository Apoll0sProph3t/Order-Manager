using System;
using System.Linq;
using Domain.Entities.BaseEntity;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Entities;

public class Order : Entity
{
    public Guid CustomerId { get; private set; }
    public Customer? Customer { get; private set; }
    public DateTime OrderDate { get; private set; }
    public Money TotalAmount { get; private set; }
    public OrderStatus Status { get; private set; }
    public List<OrderItem> Items { get; private set; } = new();

    private Order() { }

    public Order(Guid customerId)
    {
        CustomerId = customerId;
        OrderDate = DateTime.UtcNow;
        Status = OrderStatus.Pending;
    }

    public void AddItem(Product product, int quantity)
    {
        if (product is null) throw new ArgumentNullException(nameof(product));
        if (quantity <= 0) throw new InvalidOperationException("Quantidade inválida.");

        var unit = product.Price;
        var total = new Money(unit.Value * quantity);

        Items.Add(new OrderItem
        {
            OrderId = Id,
            ProductId = product.Id,
            ProductName = product.Name,
            Quantity = quantity,
            UnitPrice = unit,
            TotalPrice = total
        });

        RecalculateTotal();
    }

    public void ChangeStatus(OrderStatus newStatus)
    {
        if (Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("Pedido cancelado não pode mudar de status.");
        if (Status == OrderStatus.Pending && newStatus == OrderStatus.Shipped)
            throw new InvalidOperationException("Não é possível enviar antes de pagar.");

        Status = newStatus;
    }

    private void RecalculateTotal()
    {
        TotalAmount = new Money(Items.Sum(i => i.TotalPrice.Value));
    }
}

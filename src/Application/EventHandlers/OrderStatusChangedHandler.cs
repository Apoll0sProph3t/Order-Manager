using Application.Common.Interfaces;
using Application.DTOs;
using Application.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.EventHandlers;

public class OrderStatusChangedHandler : INotificationHandler<OrderStatusChanged>
{
    private readonly IReadModelService _read;
    private readonly IAppDbContext _db;
    public OrderStatusChangedHandler(IReadModelService read, IAppDbContext db) { _read = read; _db = db; }

    public async Task Handle(OrderStatusChanged notification, CancellationToken ct)
    {
        var order = await _db.Orders.Include(o => o.Customer).Include(o => o.Items)
            .AsNoTracking().FirstOrDefaultAsync(o => o.Id == notification.OrderId, ct);
        if (order is null) return;

        var dto = new OrderDetailDto(
            order.Id,
            new CustomerDto(order.CustomerId, order.Customer!.Name, order.Customer.Email.Value, order.Customer.Phone.Value),
            order.OrderDate,
            order.TotalAmount.Value,
            order.Status.ToString(),
            order.Items.Select(i => new OrderItemDto(i.ProductId, i.ProductName, i.Quantity, i.UnitPrice.Value, i.TotalPrice.Value)).ToList());

        await _read.UpsertOrderReadModelAsync(dto, ct);
    }
}

using Application.Common.Interfaces;
using Application.Customers.DTOs;
using Application.Orders.DTOs;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Orders.Commands;

public record CreateOrder(Guid CustomerId, IReadOnlyList<(Guid ProductId, int Quantity)> Items) : IRequest<Guid>;
public record UpdateOrderStatus(Guid OrderId, OrderStatus Status) : IRequest;
public record DeleteOrder(Guid OrderId) : IRequest;
public record ListOrders : IRequest<IReadOnlyList<OrderSummaryDto>>;
public record GetOrderDetail(Guid OrderId) : IRequest<OrderDetailDto?>;

public class OrderHandlers :
    IRequestHandler<CreateOrder, Guid>,
    IRequestHandler<UpdateOrderStatus>,
    IRequestHandler<DeleteOrder>,
    IRequestHandler<ListOrders, IReadOnlyList<OrderSummaryDto>>,
    IRequestHandler<GetOrderDetail, OrderDetailDto?>
{
    private readonly IAppDbContext _db;
    private readonly IReadModelService _read;
    public OrderHandlers(IAppDbContext db, IReadModelService read) { _db = db; _read = read; }

    public async Task<Guid> Handle(CreateOrder request, CancellationToken ct)
    {
        var customer = await _db.Customers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.CustomerId, ct)
            ?? throw new KeyNotFoundException("Customer not found");

        var products = await _db.Products.AsNoTracking()
            .Where(p => request.Items.Select(i => i.ProductId).Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, ct);

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customer.Id,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending
        };

        foreach (var item in request.Items)
        {
            if (!products.TryGetValue(item.ProductId, out var product))
                throw new KeyNotFoundException($"Product {item.ProductId} not found");

            var unit = product.Price;
            var total = unit * item.Quantity;
            order.Items.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ProductId = product.Id,
                ProductName = product.Name,
                Quantity = item.Quantity,
                UnitPrice = unit,
                TotalPrice = total
            });
        }

        order.TotalAmount = order.Items.Sum(i => i.TotalPrice);

        _db.Orders.Add(order);
        await _db.SaveChangesAsync(ct);

        var dto = new OrderDetailDto(
            order.Id,
            new CustomerDto(customer.Id, customer.Name, customer.Email, customer.Phone),
            order.OrderDate,
            order.TotalAmount,
            order.Status.ToString(),
            order.Items.Select(i => new OrderItemDto(i.ProductId, i.ProductName, i.Quantity, i.UnitPrice, i.TotalPrice)).ToList());

        await _read.UpsertOrderReadModelAsync(dto, ct);
        return order.Id;
    }

    public async Task Handle(UpdateOrderStatus request, CancellationToken ct)
    {
        var order = await _db.Orders.Include(o => o.Customer).Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, ct) ?? throw new KeyNotFoundException("Order not found");

        order.Status = request.Status;
        await _db.SaveChangesAsync(ct);

        var dto = new OrderDetailDto(
            order.Id,
            new CustomerDto(order.CustomerId, order.Customer!.Name, order.Customer.Email, order.Customer.Phone),
            order.OrderDate,
            order.TotalAmount,
            order.Status.ToString(),
            order.Items.Select(i => new OrderItemDto(i.ProductId, i.ProductName, i.Quantity, i.UnitPrice, i.TotalPrice)).ToList());

        await _read.UpsertOrderReadModelAsync(dto, ct);
    }

    public async Task Handle(DeleteOrder request, CancellationToken ct)
    {
        var order = await _db.Orders.FindAsync(new object?[] { request.OrderId }, ct) ?? throw new KeyNotFoundException("Order not found");
        _db.Orders.Remove(order);
        await _db.SaveChangesAsync(ct);
        await _read.DeleteOrderReadModelAsync(request.OrderId, ct);
    }

    public Task<IReadOnlyList<OrderSummaryDto>> Handle(ListOrders request, CancellationToken ct) =>
        _read.ListOrdersAsync(ct);

    public Task<OrderDetailDto?> Handle(GetOrderDetail request, CancellationToken ct) =>
        _read.GetOrderDetailAsync(request.OrderId, ct);
}

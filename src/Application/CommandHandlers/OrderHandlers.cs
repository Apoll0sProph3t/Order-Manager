using Application.Commands;
using Application.Common.Interfaces;
using Application.DTOs;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CommandHandlers;

public class OrderHandlers :
    IRequestHandler<CreateOrder, Guid>,
    IRequestHandler<UpdateOrderStatus>,
    IRequestHandler<DeleteOrder>
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

        var order = new Order(customer.Id);

        foreach (var item in request.Items)
        {
            if (!products.TryGetValue(item.ProductId, out var product))
                throw new KeyNotFoundException($"Product {item.ProductId} not found");

            order.AddItem(product, item.Quantity);
        }

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

        order.ChangeStatus(request.Status);
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

}

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
    private readonly IMediator _mediator;
    private readonly IOrderRepository _orders;
    public OrderHandlers(IAppDbContext db, IMediator mediator, IOrderRepository orders) { _db = db; _mediator = mediator; _orders = orders; }

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

        await _orders.AddAsync(order, ct);
        await _db.SaveChangesAsync(ct);

        await _mediator.Publish(new Application.Events.OrderCreated(order.Id), ct);
        return order.Id;
    }

    public async Task Handle(UpdateOrderStatus request, CancellationToken ct)
    {
        var order = await _orders.GetByIdWithCustomerItemsAsync(request.OrderId, ct) ?? throw new KeyNotFoundException("Order not found");

        order.ChangeStatus(request.Status);
        await _db.SaveChangesAsync(ct);

        await _mediator.Publish(new Application.Events.OrderStatusChanged(order.Id, request.Status), ct);
    }

    public async Task Handle(DeleteOrder request, CancellationToken ct)
    {
        var order = await _orders.GetByIdAsync(request.OrderId, ct) ?? throw new KeyNotFoundException("Order not found");
        await _orders.RemoveAsync(order, ct);
        await _db.SaveChangesAsync(ct);
        await _mediator.Publish(new Application.Events.OrderDeleted(request.OrderId), ct);
    }

}

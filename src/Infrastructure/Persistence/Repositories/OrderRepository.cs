using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly IAppDbContext _db;
    public OrderRepository(IAppDbContext db) => _db = db;

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken ct) =>
        await _db.Orders.FindAsync(new object?[] { id }, ct);

    public async Task<Order?> GetByIdWithCustomerItemsAsync(Guid id, CancellationToken ct) =>
        await _db.Orders.Include(o => o.Customer).Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id, ct);

    public async Task AddAsync(Order order, CancellationToken ct)
    {
        _db.Orders.Add(order);
        await Task.CompletedTask;
    }

    public async Task RemoveAsync(Order order, CancellationToken ct)
    {
        _db.Orders.Remove(order);
        await Task.CompletedTask;
    }
}

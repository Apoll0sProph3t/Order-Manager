using Application.Common.Interfaces;
using Application.DTOs;
using Domain.Enums;
using MongoDB.Driver;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.ReadModel;

public class MongoReadModelService : IReadModelService
{
    private readonly IMongoCollection<OrderDetailDto> _details;
    private readonly IAppDbContext _db;
    public MongoReadModelService(IMongoClient client, IAppDbContext db)
    {
        var dbMongo = client.GetDatabase("prova_tecnica_read");
        _details = dbMongo.GetCollection<OrderDetailDto>("orders_read");
        _db = db;
    }

    public async Task UpsertOrderReadModelAsync(OrderDetailDto order, CancellationToken ct)
    {
        try
        {
            var filter = Builders<OrderDetailDto>.Filter.Eq(x => x.Id, order.Id);
            await _details.ReplaceOneAsync(filter, order, new ReplaceOptions { IsUpsert = true }, ct);
        }
        catch
        {
        }
    }

    public async Task DeleteOrderReadModelAsync(Guid orderId, CancellationToken ct)
    {
        try
        {
            await _details.DeleteOneAsync(Builders<OrderDetailDto>.Filter.Eq(x => x.Id, orderId), ct);
        }
        catch
        {
        }
    }

    public async Task<OrderDetailDto?> GetOrderDetailAsync(Guid orderId, CancellationToken ct)
    {
        try
        {
            return await _details.Find(Builders<OrderDetailDto>.Filter.Eq(x => x.Id, orderId)).FirstOrDefaultAsync(ct);
        }
        catch
        {
            var order = await _db.Orders.Include(o => o.Customer).Include(o => o.Items)
                .AsNoTracking().FirstOrDefaultAsync(o => o.Id == orderId, ct);
            if (order == null) return null;
            return new OrderDetailDto(
                order.Id,
                new CustomerDto(order.CustomerId, order.Customer!.Name, order.Customer.Email, order.Customer.Phone),
                order.OrderDate,
                order.TotalAmount,
                order.Status.ToString(),
                order.Items.Select(i => new OrderItemDto(i.ProductId, i.ProductName, i.Quantity, i.UnitPrice, i.TotalPrice)).ToList());
        }
    }

    public async Task<IReadOnlyList<OrderSummaryDto>> ListOrdersAsync(CancellationToken ct)
    {
        try
        {
            var docs = await _details.Find(Builders<OrderDetailDto>.Filter.Empty).ToListAsync(ct);
            return docs.Select(d => new OrderSummaryDto(d.Id, d.Customer.Name, d.OrderDate, d.TotalAmount, d.Status)).ToList();
        }
        catch
        {
            return await _db.Orders.AsNoTracking()
                .Include(o => o.Customer)
                .Select(o => new OrderSummaryDto(o.Id, o.Customer!.Name, o.OrderDate, o.TotalAmount, o.Status.ToString()))
                .ToListAsync(ct);
        }
    }
}

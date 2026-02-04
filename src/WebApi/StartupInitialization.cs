using Application.Commands;
using Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace WebApi;

public static class StartupInitialization
{
    public static async Task RunAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var db = services.GetRequiredService<IAppDbContext>();
        var mediator = services.GetRequiredService<IMediator>();
        var read = services.GetRequiredService<IReadModelService>();

        var productCount = await db.Products.CountAsync();
        var hasCustomers = await db.Customers.AnyAsync();
        var hasOrders = await db.Orders.AnyAsync();

        Guid customerId;
        List<Guid> productIds = new();

        if (productCount < 3)
        {
            var needed = 3 - productCount;
            var names = new[] { "Produto A", "Produto B", "Produto C" };
            var prices = new[] { 10m, 25.5m, 5m };
            var existing = await db.Products.AsNoTracking().Select(p => p.Name).ToListAsync();
            for (int i = 0; i < names.Length && needed > 0; i++)
            {
                if (!existing.Contains(names[i]))
                {
                    var id = await mediator.Send(new CreateProduct(names[i], prices[i]));
                    productIds.Add(id);
                    needed--;
                }
            }
        }
        productIds.AddRange(await db.Products.AsNoTracking().OrderBy(p => p.Id).Select(p => p.Id).Take(3).ToListAsync());

        if (!hasCustomers)
        {
            customerId = await mediator.Send(new CreateCustomer("Cliente Demo", "cliente@demo.com", "+55 11 99999-0000"));
        }
        else
        {
            customerId = await db.Customers.AsNoTracking().OrderBy(c => c.Id).Select(c => c.Id).FirstAsync();
        }

        if (!hasOrders && productIds.Count > 0)
        {
            var items = productIds.Select(id => (id, 2)).ToList();
            await mediator.Send(new CreateOrder(customerId, items));
        }

        var allOrders = await db.Orders.AsNoTracking().Include(o => o.Customer).Include(o => o.Items).ToListAsync();
        foreach (var order in allOrders)
        {
            var dto = new Application.DTOs.OrderDetailDto(
                order.Id,
                new Application.DTOs.CustomerDto(order.CustomerId, order.Customer!.Name, order.Customer.Email, order.Customer.Phone),
                order.OrderDate,
                order.TotalAmount,
                order.Status.ToString(),
                order.Items.Select(i => new Application.DTOs.OrderItemDto(i.ProductId, i.ProductName, i.Quantity, i.UnitPrice, i.TotalPrice)).ToList());
            await read.UpsertOrderReadModelAsync(dto, CancellationToken.None);
        }
    }
}

using Application.Customers.Commands;
using Application.Products.Commands;
using Application.Orders.Commands;
using Application.Common.Interfaces;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(opt => opt.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<CreateCustomer>();
});

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
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
    productIds.AddRange(await db.Products.AsNoTracking().Select(p => p.Id).Take(3).ToListAsync());

    if (!hasCustomers)
    {
        customerId = await mediator.Send(new CreateCustomer("Cliente Demo", "cliente@demo.com", "+55 11 99999-0000"));
    }
    else
    {
        customerId = await db.Customers.AsNoTracking().Select(c => c.Id).FirstAsync();
    }

    if (!hasOrders && productIds.Count > 0)
    {
        var items = productIds.Select(id => (id, 2)).ToList();
        await mediator.Send(new CreateOrder(customerId, items));
    }

    var allOrders = await db.Orders.AsNoTracking().Include(o => o.Customer).Include(o => o.Items).ToListAsync();
    foreach (var order in allOrders)
    {
        var dto = new Application.Orders.DTOs.OrderDetailDto(
            order.Id,
            new Application.Customers.DTOs.CustomerDto(order.CustomerId, order.Customer!.Name, order.Customer.Email, order.Customer.Phone),
            order.OrderDate,
            order.TotalAmount,
            order.Status.ToString(),
            order.Items.Select(i => new Application.Orders.DTOs.OrderItemDto(i.ProductId, i.ProductName, i.Quantity, i.UnitPrice, i.TotalPrice)).ToList());
        await read.UpsertOrderReadModelAsync(dto, CancellationToken.None);
    }
}

app.Run();

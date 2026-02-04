using Application.Commands;
using Application.Common.Interfaces;
using Application.DTOs;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CommandHandlers;

public class ProductHandlers :
    IRequestHandler<CreateProduct, Guid>,
    IRequestHandler<UpdateProduct>,
    IRequestHandler<DeleteProduct>
{
    private readonly IAppDbContext _db;
    public ProductHandlers(IAppDbContext db) => _db = db;

    public async Task<Guid> Handle(CreateProduct request, CancellationToken ct)
    {
        var p = new Product { Name = request.Name, Price = new Domain.ValueObjects.Money(request.Price) };
        _db.Products.Add(p);
        await _db.SaveChangesAsync(ct);
        return p.Id;
    }

    public async Task Handle(UpdateProduct request, CancellationToken ct)
    {
        var p = await _db.Products.FindAsync(new object?[] { request.Id }, ct) ?? throw new KeyNotFoundException("Product not found");
        p.Name = request.Name; p.Price = new Domain.ValueObjects.Money(request.Price);
        await _db.SaveChangesAsync(ct);
    }

    public async Task Handle(DeleteProduct request, CancellationToken ct)
    {
        var p = await _db.Products.FindAsync(new object?[] { request.Id }, ct) ?? throw new KeyNotFoundException("Product not found");
        var hasOrderItems = await _db.OrderItems.AsNoTracking().AnyAsync(oi => oi.ProductId == request.Id, ct);
        if (hasOrderItems)
            throw new InvalidOperationException("Produto possui itens de pedido associados e não pode ser excluído.");
        _db.Products.Remove(p);
        await _db.SaveChangesAsync(ct);
    }

}

using Application.Common.Interfaces;
using Application.Products.DTOs;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Products.Commands;

public record CreateProduct(string Name, decimal Price) : IRequest<Guid>;
public record UpdateProduct(Guid Id, string Name, decimal Price) : IRequest;
public record DeleteProduct(Guid Id) : IRequest;
public record ListProducts : IRequest<IReadOnlyList<ProductDto>>;

public class ProductHandlers :
    IRequestHandler<CreateProduct, Guid>,
    IRequestHandler<UpdateProduct>,
    IRequestHandler<DeleteProduct>,
    IRequestHandler<ListProducts, IReadOnlyList<ProductDto>>
{
    private readonly IAppDbContext _db;
    public ProductHandlers(IAppDbContext db) => _db = db;

    public async Task<Guid> Handle(CreateProduct request, CancellationToken ct)
    {
        var p = new Product { Id = Guid.NewGuid(), Name = request.Name, Price = request.Price };
        _db.Products.Add(p);
        await _db.SaveChangesAsync(ct);
        return p.Id;
    }

    public async Task Handle(UpdateProduct request, CancellationToken ct)
    {
        var p = await _db.Products.FindAsync(new object?[] { request.Id }, ct) ?? throw new KeyNotFoundException("Product not found");
        p.Name = request.Name; p.Price = request.Price;
        await _db.SaveChangesAsync(ct);
    }

    public async Task Handle(DeleteProduct request, CancellationToken ct)
    {
        var p = await _db.Products.FindAsync(new object?[] { request.Id }, ct) ?? throw new KeyNotFoundException("Product not found");
        _db.Products.Remove(p);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<ProductDto>> Handle(ListProducts request, CancellationToken ct)
    {
        return await _db.Products.AsNoTracking()
            .Select(x => new ProductDto(x.Id, x.Name, x.Price))
            .ToListAsync(ct);
    }
}

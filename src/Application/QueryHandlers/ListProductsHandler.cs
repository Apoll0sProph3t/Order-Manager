namespace Application.QueryHandlers;

using Application.Common.Interfaces;
using Application.DTOs;
using Application.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class ListProductsHandler : IRequestHandler<ListProductsQuery, IReadOnlyList<ProductDto>>
{
    private readonly IAppDbContext _db;
    public ListProductsHandler(IAppDbContext db) => _db = db;

    public async Task<IReadOnlyList<ProductDto>> Handle(ListProductsQuery request, CancellationToken ct)
    {
        return await _db.Products.AsNoTracking()
            .Select(x => new ProductDto(x.Id, x.Name, x.Price))
            .ToListAsync(ct);
    }
}

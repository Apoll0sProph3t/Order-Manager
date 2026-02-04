namespace Application.QueryHandlers;

using Application.Common.Interfaces;
using Application.DTOs;
using Application.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class ListCustomersHandler : IRequestHandler<ListCustomersQuery, IReadOnlyList<CustomerDto>>
{
    private readonly IAppDbContext _db;
    public ListCustomersHandler(IAppDbContext db) => _db = db;

    public async Task<IReadOnlyList<CustomerDto>> Handle(ListCustomersQuery request, CancellationToken ct)
    {
        return await _db.Customers.AsNoTracking()
            .Select(x => new CustomerDto(x.Id, x.Name, x.Email, x.Phone))
            .ToListAsync(ct);
    }
}

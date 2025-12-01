using Application.Common.Interfaces;
using Application.Customers.DTOs;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Customers.Commands;

public record CreateCustomer(string Name, string Email, string Phone) : IRequest<Guid>;
public record UpdateCustomer(Guid Id, string Name, string Email, string Phone) : IRequest;
public record DeleteCustomer(Guid Id) : IRequest;
public record ListCustomers : IRequest<IReadOnlyList<CustomerDto>>;

public class CustomerHandlers :
    IRequestHandler<CreateCustomer, Guid>,
    IRequestHandler<UpdateCustomer>,
    IRequestHandler<DeleteCustomer>,
    IRequestHandler<ListCustomers, IReadOnlyList<CustomerDto>>
{
    private readonly IAppDbContext _db;
    public CustomerHandlers(IAppDbContext db) => _db = db;

    public async Task<Guid> Handle(CreateCustomer request, CancellationToken ct)
    {
        var c = new Customer { Id = Guid.NewGuid(), Name = request.Name, Email = request.Email, Phone = request.Phone };
        _db.Customers.Add(c);
        await _db.SaveChangesAsync(ct);
        return c.Id;
    }

    public async Task Handle(UpdateCustomer request, CancellationToken ct)
    {
        var c = await _db.Customers.FindAsync(new object?[] { request.Id }, ct) ?? throw new KeyNotFoundException("Customer not found");
        c.Name = request.Name; c.Email = request.Email; c.Phone = request.Phone;
        await _db.SaveChangesAsync(ct);
    }

    public async Task Handle(DeleteCustomer request, CancellationToken ct)
    {
        var c = await _db.Customers.FindAsync(new object?[] { request.Id }, ct) ?? throw new KeyNotFoundException("Customer not found");
        _db.Customers.Remove(c);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<CustomerDto>> Handle(ListCustomers request, CancellationToken ct)
    {
        return await _db.Customers.AsNoTracking()
            .Select(x => new CustomerDto(x.Id, x.Name, x.Email, x.Phone))
            .ToListAsync(ct);
    }
}

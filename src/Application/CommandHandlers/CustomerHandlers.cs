using Application.Commands;
using Application.Common.Interfaces;
using Application.DTOs;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CommandHandlers;

public class CustomerHandlers :
    IRequestHandler<CreateCustomer, Guid>,
    IRequestHandler<UpdateCustomer>,
    IRequestHandler<DeleteCustomer>
{
    private readonly IAppDbContext _db;
    public CustomerHandlers(IAppDbContext db) => _db = db;

    public async Task<Guid> Handle(CreateCustomer request, CancellationToken ct)
    {
        var c = new Customer { Name = request.Name, Email = new Domain.ValueObjects.Email(request.Email), Phone = new Domain.ValueObjects.Phone(request.Phone) };
        _db.Customers.Add(c);
        await _db.SaveChangesAsync(ct);
        return c.Id;
    }

    public async Task Handle(UpdateCustomer request, CancellationToken ct)
    {
        var c = await _db.Customers.FindAsync(new object?[] { request.Id }, ct) ?? throw new KeyNotFoundException("Customer not found");
        c.Name = request.Name; c.Email = new Domain.ValueObjects.Email(request.Email); c.Phone = new Domain.ValueObjects.Phone(request.Phone);
        await _db.SaveChangesAsync(ct);
    }

    public async Task Handle(DeleteCustomer request, CancellationToken ct)
    {
        var c = await _db.Customers.FindAsync(new object?[] { request.Id }, ct) ?? throw new KeyNotFoundException("Customer not found");
        var hasOrders = await _db.Orders.AsNoTracking().AnyAsync(o => o.CustomerId == request.Id, ct);
        if (hasOrders)
            throw new InvalidOperationException("Cliente possui pedidos associados e não pode ser excluído.");
        _db.Customers.Remove(c);
        await _db.SaveChangesAsync(ct);
    }

}

using Domain.Enums;
using MediatR;

namespace Application.Commands;

public record CreateOrder(Guid CustomerId, IReadOnlyList<(Guid ProductId, int Quantity)> Items) : IRequest<Guid>;
public record UpdateOrderStatus(Guid OrderId, OrderStatus Status) : IRequest;
public record DeleteOrder(Guid OrderId) : IRequest;

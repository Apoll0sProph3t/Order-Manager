namespace Application.Queries;

using Application.DTOs;
using MediatR;

public record GetOrderDetailQuery(Guid OrderId) : IRequest<OrderDetailDto?>;

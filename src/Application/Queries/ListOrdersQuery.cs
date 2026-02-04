namespace Application.Queries;

using Application.DTOs;
using MediatR;

public record ListOrdersQuery : IRequest<IReadOnlyList<OrderSummaryDto>>;

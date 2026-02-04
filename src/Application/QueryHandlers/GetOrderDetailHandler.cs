namespace Application.QueryHandlers;

using Application.DTOs;
using Application.Queries;
using Application.Common.Interfaces;
using MediatR;

public class GetOrderDetailHandler : IRequestHandler<GetOrderDetailQuery, OrderDetailDto?>
{
    private readonly IReadModelService _read;
    public GetOrderDetailHandler(IReadModelService read) => _read = read;

    public Task<OrderDetailDto?> Handle(GetOrderDetailQuery request, CancellationToken ct) =>
        _read.GetOrderDetailAsync(request.OrderId, ct);
}

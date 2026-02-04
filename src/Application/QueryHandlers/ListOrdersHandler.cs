namespace Application.QueryHandlers;

using Application.DTOs;
using Application.Queries;
using Application.Common.Interfaces;
using MediatR;

public class ListOrdersHandler : IRequestHandler<ListOrdersQuery, IReadOnlyList<OrderSummaryDto>>
{
    private readonly IReadModelService _read;
    public ListOrdersHandler(IReadModelService read) => _read = read;

    public Task<IReadOnlyList<OrderSummaryDto>> Handle(ListOrdersQuery request, CancellationToken ct) =>
        _read.ListOrdersAsync(ct);
}

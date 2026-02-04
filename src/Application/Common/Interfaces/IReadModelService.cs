using Application.DTOs;

namespace Application.Common.Interfaces;

public interface IReadModelService
{
    Task UpsertOrderReadModelAsync(OrderDetailDto order, CancellationToken ct);
    Task DeleteOrderReadModelAsync(Guid orderId, CancellationToken ct);
    Task<OrderDetailDto?> GetOrderDetailAsync(Guid orderId, CancellationToken ct);
    Task<IReadOnlyList<OrderSummaryDto>> ListOrdersAsync(CancellationToken ct);
}

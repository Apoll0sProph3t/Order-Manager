using Application.Customers.DTOs;

namespace Application.Orders.DTOs;

public record OrderDetailDto(
    Guid Id,
    CustomerDto Customer,
    DateTime OrderDate,
    decimal TotalAmount,
    string Status,
    IReadOnlyList<OrderItemDto> Items);

public record OrderSummaryDto(Guid Id, string CustomerName, DateTime OrderDate, decimal TotalAmount, string Status);

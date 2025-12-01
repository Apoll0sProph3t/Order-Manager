namespace Application.Orders.DTOs;

public record OrderItemDto(Guid ProductId, string ProductName, int Quantity, decimal UnitPrice, decimal TotalPrice);

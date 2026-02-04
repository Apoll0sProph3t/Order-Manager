using Domain.Enums;
using MediatR;

namespace Application.Events;

public record OrderStatusChanged(Guid OrderId, OrderStatus NewStatus) : INotification;

using Application.Common.Interfaces;
using Application.Events;
using MediatR;

namespace Application.EventHandlers;

public class OrderDeletedHandler : INotificationHandler<OrderDeleted>
{
    private readonly IReadModelService _read;
    public OrderDeletedHandler(IReadModelService read) => _read = read;

    public async Task Handle(OrderDeleted notification, CancellationToken ct)
    {
        await _read.DeleteOrderReadModelAsync(notification.OrderId, ct);
    }
}

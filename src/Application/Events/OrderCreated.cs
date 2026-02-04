using MediatR;

namespace Application.Events;

public record OrderCreated(Guid OrderId) : INotification;

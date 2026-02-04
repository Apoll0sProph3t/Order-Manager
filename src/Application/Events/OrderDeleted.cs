using MediatR;

namespace Application.Events;

public record OrderDeleted(Guid OrderId) : INotification;

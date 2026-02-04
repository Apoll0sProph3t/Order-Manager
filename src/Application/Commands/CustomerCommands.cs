using MediatR;

namespace Application.Commands;

public record CreateCustomer(string Name, string Email, string Phone) : IRequest<Guid>;
public record UpdateCustomer(Guid Id, string Name, string Email, string Phone) : IRequest;
public record DeleteCustomer(Guid Id) : IRequest;

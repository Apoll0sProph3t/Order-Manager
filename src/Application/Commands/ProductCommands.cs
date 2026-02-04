using MediatR;

namespace Application.Commands;

public record CreateProduct(string Name, decimal Price) : IRequest<Guid>;
public record UpdateProduct(Guid Id, string Name, decimal Price) : IRequest;
public record DeleteProduct(Guid Id) : IRequest;

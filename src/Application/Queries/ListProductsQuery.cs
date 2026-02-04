namespace Application.Queries;

using Application.DTOs;
using MediatR;

public record ListProductsQuery : IRequest<IReadOnlyList<ProductDto>>;

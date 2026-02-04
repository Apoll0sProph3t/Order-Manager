namespace Application.Queries;

using Application.DTOs;
using MediatR;

public record ListCustomersQuery : IRequest<IReadOnlyList<CustomerDto>>;

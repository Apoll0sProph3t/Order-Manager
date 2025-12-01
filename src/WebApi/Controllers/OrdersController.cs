using Application.Orders.Commands;
using Application.Orders.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    public OrdersController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderSummaryDto>>> List() =>
        Ok(await _mediator.Send(new ListOrders()));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderDetailDto?>> Get(Guid id) =>
        Ok(await _mediator.Send(new GetOrderDetail(id)));

    public record CreateOrderItem(Guid ProductId, int Quantity);
    public record CreateOrderBody(Guid CustomerId, List<CreateOrderItem> Items);

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateOrderBody body) =>
        Ok(await _mediator.Send(new CreateOrder(body.CustomerId, body.Items.Select(i => (i.ProductId, i.Quantity)).ToList())));

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateOrderStatus body)
    {
        await _mediator.Send(body with { OrderId = id });
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteOrder(id));
        return NoContent();
    }
}

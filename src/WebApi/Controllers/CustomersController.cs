using Application.Customers.Commands;
using Application.Customers.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;
    public CustomersController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CustomerDto>>> List() =>
        Ok(await _mediator.Send(new ListCustomers()));

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateCustomer cmd) =>
        Ok(await _mediator.Send(cmd));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomer body)
    {
        await _mediator.Send(body with { Id = id });
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteCustomer(id));
        return NoContent();
    }
}

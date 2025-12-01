using Application.Products.Commands;
using Application.Products.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    public ProductsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> List() =>
        Ok(await _mediator.Send(new ListProducts()));

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateProduct cmd) =>
        Ok(await _mediator.Send(cmd));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProduct body)
    {
        await _mediator.Send(body with { Id = id });
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteProduct(id));
        return NoContent();
    }
}

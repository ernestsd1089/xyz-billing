using Application.Logic;
using Application.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly SubmitOrderHandler _handler;
    private readonly IValidator<SubmitOrderRequest> _validator;

    public OrdersController(SubmitOrderHandler handler, IValidator<SubmitOrderRequest> validator)
    {
        _handler = handler;
        _validator = validator;
    }

    [HttpPost]
    public async Task<IActionResult> Submit(SubmitOrderRequest request, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(error => new { error.PropertyName, error.ErrorMessage }) });

        var receipt = await _handler.HandleAsync(request, cancellationToken);
        return Ok(receipt);
    }
}

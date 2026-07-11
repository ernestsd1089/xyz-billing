using Application.Models;
using FluentValidation;

namespace Application.Validation;

public class SubmitOrderValidator : AbstractValidator<SubmitOrderRequest>
{
    public SubmitOrderValidator()
    {
        RuleFor(request => request.OrderNumber).NotEmpty();
        RuleFor(request => request.UserId).NotEmpty();
        RuleFor(request => request.PaymentGatewayId).NotEmpty();
        RuleFor(request => request.Amount).GreaterThan(0);
    }
}

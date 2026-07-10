using Domain.Models;
using Infrastructure.Validation.Interfaces;

namespace Infrastructure.Validation;

public class AmountLimitValidator : IChargeValidator
{
    private const decimal MaxChargeableAmount = 10_000m;

    public ChargeValidationResult Validate(Order order)
    {
        if (order.Amount > MaxChargeableAmount)
            return ChargeValidationResult.Invalid($"Amount {order.Amount} exceeds the limit of {MaxChargeableAmount}.");

        return ChargeValidationResult.Valid();
    }
}

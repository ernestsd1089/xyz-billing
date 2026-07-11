using Domain.Models;

namespace Infrastructure.Validation.Interfaces;

public interface IChargeValidator
{
    ChargeValidationResult Validate(Order order);
}

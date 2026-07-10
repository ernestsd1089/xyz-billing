using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Validation.Interfaces;

namespace Infrastructure.Gateways;

public class PayPalMockGateway : IPaymentGateway
{
    private readonly IChargeValidator _validator;

    public PayPalMockGateway(IChargeValidator validator)
    {
        _validator = validator;
    }

    public string GatewayId => "paypal";

    public Task<PaymentResult> ChargeAsync(Order order, CancellationToken cancellationToken = default)
    {
        var validation = _validator.Validate(order);
        if (!validation.IsValid)
            return Task.FromResult(PaymentResult.Failure(validation.FailureReason));

        return Task.FromResult(PaymentResult.Success($"paypal_{Guid.NewGuid():N}"));
    }
}

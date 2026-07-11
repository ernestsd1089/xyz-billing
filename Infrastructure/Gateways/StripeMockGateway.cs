using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Validation.Interfaces;

namespace Infrastructure.Gateways;

public class StripeMockGateway : IPaymentGateway
{
    private readonly IChargeValidator _validator;

    public StripeMockGateway(IChargeValidator validator)
    {
        _validator = validator;
    }

    public string GatewayId => "stripe";

    public Task<PaymentResult> ChargeAsync(Order order, CancellationToken cancellationToken = default)
    {
        var validation = _validator.Validate(order);
        if (!validation.IsValid)
            return Task.FromResult(PaymentResult.Failure(validation.FailureReason));

        return Task.FromResult(PaymentResult.Success($"stripe_{Guid.NewGuid():N}"));
    }
}

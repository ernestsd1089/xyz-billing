using Domain.Interfaces;
using Domain.Models;
using Polly;

namespace Infrastructure.Resilience;

public class ResilientPaymentGateway : IPaymentGateway
{
    private readonly IPaymentGateway _inner;
    private readonly ResiliencePipeline _pipeline;

    public ResilientPaymentGateway(IPaymentGateway inner, ResiliencePipeline pipeline)
    {
        _inner = inner;
        _pipeline = pipeline;
    }

    public string GatewayId => _inner.GatewayId;

    public async Task<PaymentResult> ChargeAsync(Order order, CancellationToken cancellationToken = default)
    {
        return await _pipeline.ExecuteAsync(
            async token => await _inner.ChargeAsync(order, token),
            cancellationToken);
    }
}

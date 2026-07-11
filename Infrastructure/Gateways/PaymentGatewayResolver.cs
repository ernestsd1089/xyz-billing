using Application.Exceptions;
using Application.Interfaces;
using Domain.Interfaces;

namespace Infrastructure.Gateways;

public class PaymentGatewayResolver : IPaymentGatewayResolver
{
    private readonly IReadOnlyDictionary<string, IPaymentGateway> _gateways;

    public PaymentGatewayResolver(IEnumerable<IPaymentGateway> gateways)
    {
        _gateways = gateways.ToDictionary(gateway => gateway.GatewayId, StringComparer.OrdinalIgnoreCase);
    }

    public IPaymentGateway Resolve(string gatewayId)
    {
        if (_gateways.TryGetValue(gatewayId, out var gateway))
            return gateway;

        throw new UnknownGatewayException(gatewayId);
    }
}

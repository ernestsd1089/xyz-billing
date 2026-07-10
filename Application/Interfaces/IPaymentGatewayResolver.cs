using Domain.Interfaces;

namespace Application.Interfaces;

public interface IPaymentGatewayResolver
{
    IPaymentGateway Resolve(string gatewayId);
}

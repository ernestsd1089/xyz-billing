using Domain.Models;

namespace Domain.Interfaces;

public interface IPaymentGateway
{
    string GatewayId { get; }

    Task<PaymentResult> ChargeAsync(Order order, CancellationToken cancellationToken = default);
}

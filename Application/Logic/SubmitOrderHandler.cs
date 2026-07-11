using Application.Exceptions;
using Application.Interfaces;
using Application.Models;
using Domain.Interfaces;
using Domain.Models;

namespace Application.Logic;

public class SubmitOrderHandler
{
    private readonly IOrderRepository _orders;
    private readonly IPaymentGatewayResolver _gateways;
    private readonly IReceiptMapper _mapper;
    private readonly TimeProvider _clock;

    public SubmitOrderHandler(
        IOrderRepository orders,
        IPaymentGatewayResolver gateways,
        IReceiptMapper mapper,
        TimeProvider clock)
    {
        _orders = orders;
        _gateways = gateways;
        _mapper = mapper;
        _clock = clock;
    }

    public async Task<ReceiptResponse> HandleAsync(SubmitOrderRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await _orders.GetAsync(request.OrderNumber, cancellationToken);
        if (existing is not null)
            return _mapper.ToResponse(existing);

        var order = new Order(
            request.OrderNumber,
            request.UserId,
            request.Amount,
            request.PaymentGatewayId,
            request.Description);

        var gateway = _gateways.Resolve(order.PaymentGatewayId);
        var result = await gateway.ChargeAsync(order, cancellationToken);

        if (!result.IsSuccess)
            throw new PaymentFailedException(order.OrderNumber, result.FailureReason ?? "Payment was declined.");

        var receipt = Receipt.ForSuccessfulPayment(order, result.ConfirmationId!, _clock.GetUtcNow());
        await _orders.SaveAsync(receipt, cancellationToken);
        return _mapper.ToResponse(receipt);
    }
}

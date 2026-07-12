using Application.Exceptions;
using Application.Interfaces;
using Application.Models;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace Application.Logic;

public class SubmitOrderHandler
{
    private readonly IOrderRepository _orders;
    private readonly IPaymentGatewayResolver _gateways;
    private readonly IReceiptMapper _mapper;
    private readonly IOrderLock _orderLock;
    private readonly TimeProvider _clock;
    private readonly ILogger<SubmitOrderHandler> _logger;

    public SubmitOrderHandler(
        IOrderRepository orders,
        IPaymentGatewayResolver gateways,
        IReceiptMapper mapper,
        IOrderLock orderLock,
        TimeProvider clock,
        ILogger<SubmitOrderHandler> logger)
    {
        _orders = orders;
        _gateways = gateways;
        _mapper = mapper;
        _orderLock = orderLock;
        _clock = clock;
        _logger = logger;
    }

    public async Task<ReceiptResponse> HandleAsync(SubmitOrderRequest request, CancellationToken cancellationToken = default)
    {
        using var scope = _logger.BeginScope("OrderNumber:{OrderNumber}", request.OrderNumber);

        _logger.LogInformation("Processing order for gateway {GatewayId}.", request.PaymentGatewayId);

        using var orderLock = await _orderLock.AcquireAsync(request.OrderNumber, cancellationToken);

        var existing = await _orders.GetAsync(request.OrderNumber, cancellationToken);
        if (existing is not null)
        {
            _logger.LogInformation("Order was already processed; returning the existing receipt.");
            return _mapper.ToResponse(existing);
        }

        var order = new Order(
            request.OrderNumber,
            request.UserId,
            request.Amount,
            request.PaymentGatewayId,
            request.Description);

        var gateway = _gateways.Resolve(order.PaymentGatewayId);
        var result = await gateway.ChargeAsync(order, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Payment was declined: {Reason}", result.FailureReason);
            throw new PaymentFailedException(order.OrderNumber, result.FailureReason ?? "Payment was declined.");
        }

        var confirmationId = result.ConfirmationId
            ?? throw new InvalidOperationException("Gateway reported success without a confirmation id.");
        var receipt = Receipt.ForSuccessfulPayment(order, confirmationId, _clock.GetUtcNow());
        await _orders.SaveAsync(receipt, cancellationToken);
        _logger.LogInformation("Payment succeeded with confirmation {ConfirmationId}.", receipt.ConfirmationId);
        return _mapper.ToResponse(receipt);
    }
}

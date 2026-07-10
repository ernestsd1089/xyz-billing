using Application.Exceptions;
using Application.Interfaces;
using Application.Logic;
using Application.Mapping;
using Application.Models;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Time.Testing;

namespace UnitTests.Application;

public class SubmitOrderHandlerTests
{
    private static readonly DateTimeOffset FixedTime = new(2026, 7, 11, 12, 0, 0, TimeSpan.Zero);

    private static SubmitOrderRequest ValidRequest() => new()
    {
        OrderNumber = "ORD-1",
        UserId = "user-1",
        Amount = 49.99m,
        PaymentGatewayId = "stripe",
        Description = "a note"
    };

    [Fact]
    public async Task SuccessfulPayment_ChargesSavesAndReturnsReceipt()
    {
        var orders = new FakeOrderRepository();
        var gateway = new FakeGateway { Result = PaymentResult.Success("conf-123") };
        var handler = NewHandler(orders, gateway);

        var response = await handler.HandleAsync(ValidRequest());

        Assert.Equal("conf-123", response.ConfirmationId);
        Assert.Equal(FixedTime, response.Timestamp);
        Assert.Equal(1, gateway.ChargeCount);
        Assert.Equal(1, orders.SaveCount);
    }

    [Fact]
    public async Task DeclinedPayment_ThrowsAndDoesNotSave()
    {
        var orders = new FakeOrderRepository();
        var gateway = new FakeGateway { Result = PaymentResult.Failure("insufficient funds") };
        var handler = NewHandler(orders, gateway);

        await Assert.ThrowsAsync<PaymentFailedException>(() => handler.HandleAsync(ValidRequest()));

        Assert.Equal(0, orders.SaveCount);
    }

    [Fact]
    public async Task DuplicateOrder_ReturnsExistingReceiptWithoutCharging()
    {
        var orders = new FakeOrderRepository();
        orders.Store["ORD-1"] = new Receipt("ORD-1", 49.99m, FixedTime, "conf-original", PaymentStatus.Succeeded);
        var gateway = new FakeGateway();
        var handler = NewHandler(orders, gateway);

        var response = await handler.HandleAsync(ValidRequest());

        Assert.Equal("conf-original", response.ConfirmationId);
        Assert.Equal(0, gateway.ChargeCount);
        Assert.Equal(0, orders.SaveCount);
    }

    private static SubmitOrderHandler NewHandler(IOrderRepository orders, IPaymentGateway gateway)
    {
        var clock = new FakeTimeProvider(FixedTime);
        return new SubmitOrderHandler(orders, new FakeResolver(gateway), new ReceiptMapper(), clock);
    }

    private class FakeOrderRepository : IOrderRepository
    {
        public Dictionary<string, Receipt> Store { get; } = new();
        public int SaveCount { get; private set; }

        public Task<Receipt?> GetAsync(string orderNumber, CancellationToken cancellationToken = default)
            => Task.FromResult(Store.TryGetValue(orderNumber, out var receipt) ? receipt : null);

        public Task SaveAsync(Receipt receipt, CancellationToken cancellationToken = default)
        {
            Store[receipt.OrderNumber] = receipt;
            SaveCount++;
            return Task.CompletedTask;
        }
    }

    private class FakeGateway : IPaymentGateway
    {
        public string GatewayId => "fake";
        public PaymentResult Result { get; set; } = PaymentResult.Success("conf-default");
        public int ChargeCount { get; private set; }

        public Task<PaymentResult> ChargeAsync(Order order, CancellationToken cancellationToken = default)
        {
            ChargeCount++;
            return Task.FromResult(Result);
        }
    }

    private class FakeResolver : IPaymentGatewayResolver
    {
        private readonly IPaymentGateway _gateway;

        public FakeResolver(IPaymentGateway gateway) => _gateway = gateway;

        public IPaymentGateway Resolve(string gatewayId) => _gateway;
    }
}

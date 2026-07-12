using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Resilience;

namespace UnitTests.Infrastructure;

public class ResilientPaymentGatewayTests
{
    private static Order NewOrder() => new("ORD-1", "user-1", 49.99m, "flaky");

    private static ResilientPaymentGateway Wrap(IPaymentGateway inner) =>
        new(inner, PaymentRetryPipeline.Create());

    [Fact]
    public async Task TransientFailure_IsRetriedUntilItSucceeds()
    {
        var flaky = new FlakyGateway(failFirstAttempts: 1);
        var gateway = Wrap(flaky);

        var result = await gateway.ChargeAsync(NewOrder());

        Assert.True(result.IsSuccess);
        Assert.Equal(2, flaky.ChargeCount);
    }

    [Fact]
    public async Task Decline_IsNotRetried()
    {
        var flaky = new FlakyGateway(failFirstAttempts: 0) { FinalResult = PaymentResult.Failure("declined") };
        var gateway = Wrap(flaky);

        var result = await gateway.ChargeAsync(NewOrder());

        Assert.False(result.IsSuccess);
        Assert.Equal(1, flaky.ChargeCount);
    }

    [Fact]
    public async Task Success_IsNotRetried()
    {
        var flaky = new FlakyGateway(failFirstAttempts: 0);
        var gateway = Wrap(flaky);

        var result = await gateway.ChargeAsync(NewOrder());

        Assert.True(result.IsSuccess);
        Assert.Equal(1, flaky.ChargeCount);
    }

    [Fact]
    public void GatewayId_DelegatesToInner()
    {
        var gateway = Wrap(new FlakyGateway(failFirstAttempts: 0));

        Assert.Equal("flaky", gateway.GatewayId);
    }

    private class FlakyGateway : IPaymentGateway
    {
        private readonly int _failFirstAttempts;

        public FlakyGateway(int failFirstAttempts)
        {
            _failFirstAttempts = failFirstAttempts;
        }

        public int ChargeCount { get; private set; }
        public PaymentResult FinalResult { get; set; } = PaymentResult.Success("conf-1");
        public string GatewayId => "flaky";

        public Task<PaymentResult> ChargeAsync(Order order, CancellationToken cancellationToken = default)
        {
            ChargeCount++;
            if (ChargeCount <= _failFirstAttempts)
            {
                throw new TransientGatewayException("transient blip");
            }

            return Task.FromResult(FinalResult);
        }
    }
}

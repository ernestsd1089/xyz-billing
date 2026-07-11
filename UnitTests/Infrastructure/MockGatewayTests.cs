using Domain.Models;
using Infrastructure.Gateways;
using Infrastructure.Validation;
using Infrastructure.Validation.Interfaces;

namespace UnitTests.Infrastructure;

public class MockGatewayTests
{
    [Fact]
    public void Gateways_HaveExpectedIds()
    {
        Assert.Equal("stripe", new StripeMockGateway(PassingValidator()).GatewayId);
        Assert.Equal("paypal", new PayPalMockGateway(PassingValidator()).GatewayId);
    }

    [Fact]
    public async Task StripeGateway_WhenValidationPasses_Succeeds()
    {
        var result = await new StripeMockGateway(PassingValidator()).ChargeAsync(NewOrder());

        Assert.True(result.IsSuccess);
        Assert.StartsWith("stripe_", result.ConfirmationId);
    }

    [Fact]
    public async Task StripeGateway_WhenValidationFails_DeclinesWithReason()
    {
        var result = await new StripeMockGateway(FailingValidator("declined")).ChargeAsync(NewOrder());

        Assert.False(result.IsSuccess);
        Assert.Equal("declined", result.FailureReason);
    }

    [Fact]
    public async Task PayPalGateway_WhenValidationPasses_Succeeds()
    {
        var result = await new PayPalMockGateway(PassingValidator()).ChargeAsync(NewOrder());

        Assert.True(result.IsSuccess);
        Assert.StartsWith("paypal_", result.ConfirmationId);
    }

    [Fact]
    public async Task PayPalGateway_WhenValidationFails_DeclinesWithReason()
    {
        var result = await new PayPalMockGateway(FailingValidator("declined")).ChargeAsync(NewOrder());

        Assert.False(result.IsSuccess);
        Assert.Equal("declined", result.FailureReason);
    }

    private static Order NewOrder() => new("ORD-1", "user-1", 49.99m, "stripe");

    private static IChargeValidator PassingValidator() => new StubValidator(ChargeValidationResult.Valid());

    private static IChargeValidator FailingValidator(string reason) => new StubValidator(ChargeValidationResult.Invalid(reason));

    private class StubValidator : IChargeValidator
    {
        private readonly ChargeValidationResult _result;

        public StubValidator(ChargeValidationResult result) => _result = result;

        public ChargeValidationResult Validate(Order order) => _result;
    }
}

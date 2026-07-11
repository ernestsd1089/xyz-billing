using Application.Exceptions;
using Domain.Interfaces;
using Infrastructure.Gateways;
using Infrastructure.Validation;

namespace UnitTests.Infrastructure;

public class PaymentGatewayResolverTests
{
    private static PaymentGatewayResolver NewResolver()
    {
        var validator = new AmountLimitValidator();
        return new(new IPaymentGateway[] { new StripeMockGateway(validator), new PayPalMockGateway(validator) });
    }

    [Fact]
    public void Resolve_KnownGatewayId_ReturnsMatchingGateway()
    {
        var gateway = NewResolver().Resolve("paypal");

        Assert.Equal("paypal", gateway.GatewayId);
    }

    [Fact]
    public void Resolve_IsCaseInsensitive()
    {
        var gateway = NewResolver().Resolve("STRIPE");

        Assert.Equal("stripe", gateway.GatewayId);
    }

    [Fact]
    public void Resolve_UnknownGatewayId_ThrowsUnknownGatewayException()
    {
        Assert.Throws<UnknownGatewayException>(() => NewResolver().Resolve("bitcoin"));
    }
}

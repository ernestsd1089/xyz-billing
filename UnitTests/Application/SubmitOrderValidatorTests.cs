using Application.Models;
using Application.Validation;

namespace UnitTests.Application;

public class SubmitOrderValidatorTests
{
    private readonly SubmitOrderValidator _validator = new();

    private static SubmitOrderRequest ValidRequest() => new()
    {
        OrderNumber = "ORD-1",
        UserId = "user-1",
        Amount = 49.99m,
        PaymentGatewayId = "stripe",
        Description = "a note"
    };

    [Fact]
    public void ValidRequest_PassesValidation()
    {
        var result = _validator.Validate(ValidRequest());

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void MissingOrderNumber_FailsValidation(string orderNumber)
    {
        var request = ValidRequest();
        request.OrderNumber = orderNumber;

        var result = _validator.Validate(request);

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(SubmitOrderRequest.OrderNumber));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void MissingUserId_FailsValidation(string userId)
    {
        var request = ValidRequest();
        request.UserId = userId;

        var result = _validator.Validate(request);

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(SubmitOrderRequest.UserId));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void MissingPaymentGatewayId_FailsValidation(string gatewayId)
    {
        var request = ValidRequest();
        request.PaymentGatewayId = gatewayId;

        var result = _validator.Validate(request);

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(SubmitOrderRequest.PaymentGatewayId));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.01)]
    public void NonPositiveAmount_FailsValidation(double amount)
    {
        var request = ValidRequest();
        request.Amount = (decimal)amount;

        var result = _validator.Validate(request);

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(SubmitOrderRequest.Amount));
    }
}

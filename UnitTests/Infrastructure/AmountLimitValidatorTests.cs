using Domain.Models;
using Infrastructure.Validation;

namespace UnitTests.Infrastructure;

public class AmountLimitValidatorTests
{
    private readonly AmountLimitValidator _validator = new();

    [Fact]
    public void Validate_AmountWithinLimit_IsValid()
    {
        var result = _validator.Validate(NewOrder(49.99m));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_AmountAboveLimit_IsInvalidWithReason()
    {
        var result = _validator.Validate(NewOrder(10_001m));

        Assert.False(result.IsValid);
        Assert.False(string.IsNullOrWhiteSpace(result.FailureReason));
    }

    private static Order NewOrder(decimal amount) => new("ORD-1", "user-1", amount, "stripe");
}

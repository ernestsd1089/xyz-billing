using Domain.Enums;
using Domain.Models;

namespace UnitTests.Domain;

public class ReceiptTests
{
    [Fact]
    public void ForSuccessfulPayment_BuildsSucceededReceiptFromOrder()
    {
        var order = new Order("ORD-1", "user-1", 49.99m, "stripe");
        var when = DateTimeOffset.UnixEpoch;

        var receipt = Receipt.ForSuccessfulPayment(order, "conf-123", when);

        Assert.Equal("ORD-1", receipt.OrderNumber);
        Assert.Equal(order.Amount, receipt.Amount);
        Assert.Equal("conf-123", receipt.ConfirmationId);
        Assert.Equal(when, receipt.Timestamp);
        Assert.Equal(PaymentStatus.Succeeded, receipt.Status);
    }
}

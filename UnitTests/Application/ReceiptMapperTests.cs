using Application.Mapping;
using Domain.Enums;
using Domain.Models;

namespace UnitTests.Application;

public class ReceiptMapperTests
{
    [Fact]
    public void ToResponse_MapsAllFields()
    {
        var mapper = new ReceiptMapper();
        var timestamp = new DateTimeOffset(2026, 7, 11, 12, 0, 0, TimeSpan.Zero);
        var receipt = new Receipt("ORD-1", 49.99m, timestamp, "conf-123", PaymentStatus.Succeeded);

        var response = mapper.ToResponse(receipt);

        Assert.Equal("ORD-1", response.OrderNumber);
        Assert.Equal(49.99m, response.Amount);
        Assert.Equal(timestamp, response.Timestamp);
        Assert.Equal("conf-123", response.ConfirmationId);
        Assert.Equal("Succeeded", response.Status);
    }
}

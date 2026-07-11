using System.Net;
using System.Net.Http.Json;
using Application.Models;
using Microsoft.AspNetCore.Mvc.Testing;

namespace IntegrationTests;

public class OrdersApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public OrdersApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    private static SubmitOrderRequest ValidRequest(string orderNumber) => new()
    {
        OrderNumber = orderNumber,
        UserId = "user-1",
        Amount = 49.99m,
        PaymentGatewayId = "stripe",
        Description = "widget"
    };

    [Fact]
    public async Task Submit_ValidOrder_Returns200WithReceipt()
    {
        var response = await _client.PostAsJsonAsync("/api/orders", ValidRequest("int-ok"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var receipt = await response.Content.ReadFromJsonAsync<ReceiptResponse>();
        Assert.NotNull(receipt);
        Assert.Equal("int-ok", receipt.OrderNumber);
        Assert.Equal("Succeeded", receipt.Status);
        Assert.False(string.IsNullOrWhiteSpace(receipt.ConfirmationId));
    }

    [Fact]
    public async Task Submit_SameOrderTwice_ReturnsSameReceiptWithoutDoubleCharge()
    {
        var request = ValidRequest("int-dup");

        var first = await _client.PostAsJsonAsync("/api/orders", request);
        var second = await _client.PostAsJsonAsync("/api/orders", request);

        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(HttpStatusCode.OK, second.StatusCode);

        var firstReceipt = await first.Content.ReadFromJsonAsync<ReceiptResponse>();
        var secondReceipt = await second.Content.ReadFromJsonAsync<ReceiptResponse>();
        Assert.NotNull(firstReceipt);
        Assert.NotNull(secondReceipt);
        Assert.Equal(firstReceipt.ConfirmationId, secondReceipt.ConfirmationId);
        Assert.Equal(firstReceipt.Timestamp, secondReceipt.Timestamp);
    }

    [Fact]
    public async Task Submit_AmountOverLimit_Returns402()
    {
        var request = ValidRequest("int-over");
        request.Amount = 25_000m;

        var response = await _client.PostAsJsonAsync("/api/orders", request);

        Assert.Equal(HttpStatusCode.PaymentRequired, response.StatusCode);
    }

    [Fact]
    public async Task Submit_UnknownGateway_Returns400()
    {
        var request = ValidRequest("int-unknown");
        request.PaymentGatewayId = "bitcoin";

        var response = await _client.PostAsJsonAsync("/api/orders", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Submit_InvalidPayload_Returns400()
    {
        var request = new SubmitOrderRequest
        {
            OrderNumber = "",
            UserId = "",
            Amount = 0,
            PaymentGatewayId = ""
        };

        var response = await _client.PostAsJsonAsync("/api/orders", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}

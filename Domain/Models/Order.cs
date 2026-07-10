namespace Domain.Models;

public class Order
{
    public string OrderNumber { get; }
    public string UserId { get; }
    public decimal Amount { get; }
    public string PaymentGatewayId { get; }
    public string? Description { get; }

    public Order(
        string orderNumber,
        string userId,
        decimal amount,
        string paymentGatewayId,
        string? description = null)
    {
        OrderNumber = orderNumber;
        UserId = userId;
        Amount = amount;
        PaymentGatewayId = paymentGatewayId;
        Description = description;
    }
}

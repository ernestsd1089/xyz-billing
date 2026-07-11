namespace Application.Models;

public class SubmitOrderRequest
{
    public string OrderNumber { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string PaymentGatewayId { get; set; } = string.Empty;
    public string? Description { get; set; }
}

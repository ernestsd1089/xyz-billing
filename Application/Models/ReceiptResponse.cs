namespace Application.Models;

public class ReceiptResponse
{
    public string OrderNumber { get; }
    public decimal Amount { get; }
    public DateTimeOffset Timestamp { get; }
    public string ConfirmationId { get; }
    public string Status { get; }

    public ReceiptResponse(
        string orderNumber,
        decimal amount,
        DateTimeOffset timestamp,
        string confirmationId,
        string status)
    {
        OrderNumber = orderNumber;
        Amount = amount;
        Timestamp = timestamp;
        ConfirmationId = confirmationId;
        Status = status;
    }
}

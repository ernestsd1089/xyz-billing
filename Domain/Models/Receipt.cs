using Domain.Enums;

namespace Domain.Models;

public class Receipt
{
    public string OrderNumber { get; }
    public decimal Amount { get; }
    public DateTimeOffset Timestamp { get; }
    public string ConfirmationId { get; }
    public PaymentStatus Status { get; }

    public Receipt(
        string orderNumber,
        decimal amount,
        DateTimeOffset timestamp,
        string confirmationId,
        PaymentStatus status)
    {
        OrderNumber = orderNumber;
        Amount = amount;
        Timestamp = timestamp;
        ConfirmationId = confirmationId;
        Status = status;
    }

    public static Receipt ForSuccessfulPayment(Order order, string confirmationId, DateTimeOffset timestamp)
    {
        return new Receipt(order.OrderNumber, order.Amount, timestamp, confirmationId, PaymentStatus.Succeeded);
    }
}

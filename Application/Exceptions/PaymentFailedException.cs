namespace Application.Exceptions;

public class PaymentFailedException : Exception
{
    public string OrderNumber { get; }
    public string Reason { get; }

    public PaymentFailedException(string orderNumber, string reason)
        : base($"Payment failed for order '{orderNumber}': {reason}")
    {
        OrderNumber = orderNumber;
        Reason = reason;
    }
}

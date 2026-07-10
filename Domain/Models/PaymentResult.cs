namespace Domain.Models;

public class PaymentResult
{
    public bool IsSuccess { get; }
    public string? ConfirmationId { get; }
    public string? FailureReason { get; }

    private PaymentResult(bool isSuccess, string? confirmationId, string? failureReason)
    {
        IsSuccess = isSuccess;
        ConfirmationId = confirmationId;
        FailureReason = failureReason;
    }

    public static PaymentResult Success(string confirmationId) =>
        new(true, confirmationId, null);

    public static PaymentResult Failure(string reason) =>
        new(false, null, reason);
}

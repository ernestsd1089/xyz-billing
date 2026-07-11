namespace Infrastructure.Validation;

public class ChargeValidationResult
{
    public bool IsValid { get; }
    public string FailureReason { get; }

    private ChargeValidationResult(bool isValid, string failureReason)
    {
        IsValid = isValid;
        FailureReason = failureReason;
    }

    public static ChargeValidationResult Valid() => new(true, string.Empty);

    public static ChargeValidationResult Invalid(string reason) => new(false, reason);
}

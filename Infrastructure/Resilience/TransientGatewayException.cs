namespace Infrastructure.Resilience;

public class TransientGatewayException : Exception
{
    public TransientGatewayException(string message) : base(message)
    {
    }
}

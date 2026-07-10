namespace Application.Exceptions;

public class UnknownGatewayException : Exception
{
    public string GatewayId { get; }

    public UnknownGatewayException(string gatewayId)
        : base($"No payment gateway is registered for id '{gatewayId}'.")
    {
        GatewayId = gatewayId;
    }
}

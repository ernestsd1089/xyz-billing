using Application.Interfaces;
using Application.Models;
using Domain.Models;

namespace Application.Mapping;

public class ReceiptMapper : IReceiptMapper
{
    public ReceiptResponse ToResponse(Receipt receipt)
    {
        return new ReceiptResponse(
            receipt.OrderNumber,
            receipt.Amount,
            receipt.Timestamp,
            receipt.ConfirmationId,
            receipt.Status.ToString());
    }
}

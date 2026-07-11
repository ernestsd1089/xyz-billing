using Application.Models;
using Domain.Models;

namespace Application.Interfaces;

public interface IReceiptMapper
{
    ReceiptResponse ToResponse(Receipt receipt);
}

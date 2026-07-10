using Domain.Models;

namespace Domain.Interfaces;

public interface IOrderRepository
{
    Task<Receipt?> GetAsync(string orderNumber, CancellationToken cancellationToken = default);

    Task SaveAsync(Receipt receipt, CancellationToken cancellationToken = default);
}

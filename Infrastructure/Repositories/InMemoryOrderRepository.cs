using System.Collections.Concurrent;
using Domain.Interfaces;
using Domain.Models;

namespace Infrastructure.Repositories;

public class InMemoryOrderRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<string, Receipt> _receipts = new();

    public Task<Receipt?> GetAsync(string orderNumber, CancellationToken cancellationToken = default)
    {
        _receipts.TryGetValue(orderNumber, out var receipt);
        return Task.FromResult<Receipt?>(receipt);
    }

    public Task SaveAsync(Receipt receipt, CancellationToken cancellationToken = default)
    {
        _receipts.TryAdd(receipt.OrderNumber, receipt);
        return Task.CompletedTask;
    }
}

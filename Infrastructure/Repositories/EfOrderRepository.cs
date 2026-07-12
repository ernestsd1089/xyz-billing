using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class EfOrderRepository : IOrderRepository
{
    private readonly BillingDbContext _db;

    public EfOrderRepository(BillingDbContext db)
    {
        _db = db;
    }

    public async Task<Receipt?> GetAsync(string orderNumber, CancellationToken cancellationToken = default)
    {
        return await _db.Receipts
            .AsNoTracking()
            .FirstOrDefaultAsync(receipt => receipt.OrderNumber == orderNumber, cancellationToken);
    }

    public async Task SaveAsync(Receipt receipt, CancellationToken cancellationToken = default)
    {
        var alreadyStored = await _db.Receipts
            .AnyAsync(existing => existing.OrderNumber == receipt.OrderNumber, cancellationToken);
        if (alreadyStored)
        {
            return;
        }

        _db.Receipts.Add(receipt);
        await _db.SaveChangesAsync(cancellationToken);
    }
}

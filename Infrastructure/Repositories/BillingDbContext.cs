using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class BillingDbContext : DbContext
{
    public BillingDbContext(DbContextOptions<BillingDbContext> options) : base(options)
    {
    }

    public DbSet<Receipt> Receipts => Set<Receipt>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Receipt>(entity =>
        {
            entity.HasKey(receipt => receipt.OrderNumber);
            entity.Property(receipt => receipt.Amount);
            entity.Property(receipt => receipt.Timestamp);
            entity.Property(receipt => receipt.ConfirmationId);
            entity.Property(receipt => receipt.Status);
        });
    }
}

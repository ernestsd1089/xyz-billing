using Domain.Enums;
using Domain.Models;
using Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace UnitTests.Infrastructure;

public class EfOrderRepositoryTests
{
    private static Receipt NewReceipt(string orderNumber, string confirmationId) =>
        new(orderNumber, 49.99m, DateTimeOffset.UnixEpoch, confirmationId, PaymentStatus.Succeeded);

    private static BillingDbContext NewInMemoryContext(SqliteConnection connection)
    {
        connection.Open();
        var options = new DbContextOptionsBuilder<BillingDbContext>().UseSqlite(connection).Options;
        var context = new BillingDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    [Fact]
    public async Task GetAsync_UnknownOrder_ReturnsNull()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        using var context = NewInMemoryContext(connection);
        var repository = new EfOrderRepository(context);

        var receipt = await repository.GetAsync("missing");

        Assert.Null(receipt);
    }

    [Fact]
    public async Task SaveThenGet_ReturnsSavedReceipt()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        using var context = NewInMemoryContext(connection);
        var repository = new EfOrderRepository(context);

        await repository.SaveAsync(NewReceipt("ORD-1", "conf-1"));
        var receipt = await repository.GetAsync("ORD-1");

        Assert.NotNull(receipt);
        Assert.Equal("conf-1", receipt.ConfirmationId);
    }

    [Fact]
    public async Task SaveAsync_SameOrderTwice_KeepsFirstReceipt()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        using var context = NewInMemoryContext(connection);
        var repository = new EfOrderRepository(context);

        await repository.SaveAsync(NewReceipt("ORD-1", "conf-first"));
        await repository.SaveAsync(NewReceipt("ORD-1", "conf-second"));
        var receipt = await repository.GetAsync("ORD-1");

        Assert.NotNull(receipt);
        Assert.Equal("conf-first", receipt.ConfirmationId);
    }
}

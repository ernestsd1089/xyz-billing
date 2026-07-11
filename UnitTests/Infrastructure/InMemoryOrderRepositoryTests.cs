using Domain.Enums;
using Domain.Models;
using Infrastructure.Repositories;

namespace UnitTests.Infrastructure;

public class InMemoryOrderRepositoryTests
{
    private static Receipt NewReceipt(string orderNumber, string confirmationId) =>
        new(orderNumber, 49.99m, DateTimeOffset.UnixEpoch, confirmationId, PaymentStatus.Succeeded);

    [Fact]
    public async Task GetAsync_UnknownOrder_ReturnsNull()
    {
        var repository = new InMemoryOrderRepository();

        var receipt = await repository.GetAsync("missing");

        Assert.Null(receipt);
    }

    [Fact]
    public async Task SaveThenGet_ReturnsSavedReceipt()
    {
        var repository = new InMemoryOrderRepository();
        await repository.SaveAsync(NewReceipt("ORD-1", "conf-1"));

        var receipt = await repository.GetAsync("ORD-1");

        Assert.NotNull(receipt);
        Assert.Equal("conf-1", receipt.ConfirmationId);
    }

    [Fact]
    public async Task SaveAsync_SameOrderTwice_KeepsFirstReceipt()
    {
        var repository = new InMemoryOrderRepository();
        await repository.SaveAsync(NewReceipt("ORD-1", "conf-first"));
        await repository.SaveAsync(NewReceipt("ORD-1", "conf-second"));

        var receipt = await repository.GetAsync("ORD-1");

        Assert.NotNull(receipt);
        Assert.Equal("conf-first", receipt.ConfirmationId);
    }
}

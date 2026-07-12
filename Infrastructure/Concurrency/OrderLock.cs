using Application.Interfaces;

namespace Infrastructure.Concurrency;

public class OrderLock : IOrderLock
{
    private readonly SemaphoreSlim[] _stripes;

    public OrderLock()
    {
        _stripes = new SemaphoreSlim[32];
        for (var i = 0; i < _stripes.Length; i++)
        {
            _stripes[i] = new SemaphoreSlim(1, 1);
        }
    }

    public async Task<IDisposable> AcquireAsync(string orderNumber, CancellationToken cancellationToken = default)
    {
        var stripe = _stripes[StripeIndexFor(orderNumber)];
        await stripe.WaitAsync(cancellationToken);
        return new Release(stripe);
    }

    private int StripeIndexFor(string orderNumber)
    {
        return (orderNumber.GetHashCode() & int.MaxValue) % _stripes.Length;
    }

    private class Release : IDisposable
    {
        private readonly SemaphoreSlim _stripe;

        public Release(SemaphoreSlim stripe)
        {
            _stripe = stripe;
        }

        public void Dispose()
        {
            _stripe.Release();
        }
    }
}

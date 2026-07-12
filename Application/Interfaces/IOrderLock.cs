namespace Application.Interfaces;

public interface IOrderLock
{
    Task<IDisposable> AcquireAsync(string orderNumber, CancellationToken cancellationToken = default);
}

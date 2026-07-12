using Polly;
using Polly.Retry;

namespace Infrastructure.Resilience;

public static class PaymentRetryPipeline
{
    public static ResiliencePipeline Create()
    {
        return new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<TransientGatewayException>(),
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromMilliseconds(100),
                BackoffType = DelayBackoffType.Constant
            })
            .Build();
    }
}

using Application.Interfaces;
using Domain.Interfaces;
using Infrastructure.Concurrency;
using Infrastructure.Gateways;
using Infrastructure.Repositories;
using Infrastructure.Resilience;
using Infrastructure.Validation;
using Infrastructure.Validation.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Polly;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<BillingDbContext>(options => options.UseSqlite(connectionString));
        services.AddScoped<IOrderRepository, EfOrderRepository>();
        services.AddSingleton<IOrderLock, OrderLock>();
        services.AddSingleton<IChargeValidator, AmountLimitValidator>();

        services.AddSingleton(PaymentRetryPipeline.Create());
        services.AddSingleton<StripeMockGateway>();
        services.AddSingleton<PayPalMockGateway>();
        services.AddSingleton<IPaymentGateway>(provider =>
            new ResilientPaymentGateway(provider.GetRequiredService<StripeMockGateway>(), provider.GetRequiredService<ResiliencePipeline>()));
        services.AddSingleton<IPaymentGateway>(provider =>
            new ResilientPaymentGateway(provider.GetRequiredService<PayPalMockGateway>(), provider.GetRequiredService<ResiliencePipeline>()));

        services.AddSingleton<IPaymentGatewayResolver, PaymentGatewayResolver>();
        return services;
    }
}

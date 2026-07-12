using Application.Interfaces;
using Domain.Interfaces;
using Infrastructure.Gateways;
using Infrastructure.Repositories;
using Infrastructure.Validation;
using Infrastructure.Validation.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<BillingDbContext>(options => options.UseSqlite(connectionString));
        services.AddScoped<IOrderRepository, EfOrderRepository>();
        services.AddSingleton<IChargeValidator, AmountLimitValidator>();
        services.AddSingleton<IPaymentGateway, StripeMockGateway>();
        services.AddSingleton<IPaymentGateway, PayPalMockGateway>();
        services.AddSingleton<IPaymentGatewayResolver, PaymentGatewayResolver>();
        return services;
    }
}

using Application.Interfaces;
using Application.Logic;
using Application.Mapping;
using Application.Models;
using Application.Validation;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<SubmitOrderHandler>();
        services.AddScoped<IReceiptMapper, ReceiptMapper>();
        services.AddScoped<IValidator<SubmitOrderRequest>, SubmitOrderValidator>();
        return services;
    }
}

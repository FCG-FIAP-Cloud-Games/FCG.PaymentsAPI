using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using FCG.Payments.Application.Abstractions.Repositories;
using FCG.Payments.Infrastructure.Data.EF.Context;
using FCG.Payments.Infrastructure.Repositories;
using FCG.Payments.Application.Messaging;
using FCG.Payments.Infrastructure.Messaging;

namespace FCG.Payments.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddPaymentsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PaymentsDatabase") ?? string.Empty;

        services.AddDbContext<PaymentsDbContext>(options =>
            options.UseNpgsql(connectionString, postgres =>
                postgres.MigrationsAssembly(typeof(PaymentsDbContext).Assembly.FullName)));
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IOrderPlacedEventProcessor, OrderPlacedEventProcessor>();
        return services;
    }
}

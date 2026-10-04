using Microsoft.Extensions.DependencyInjection;

namespace FCG.Payments.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddPaymentsApplication(this IServiceCollection services)
    {
        // Application use cases, consumers and processing abstractions will be registered here.
        return services;
    }
}

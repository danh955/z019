namespace z019.EodHistoricalData;

using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEodHistoricalDataService(this IServiceCollection services, EodHDClientOptions options)
    {
        services.AddSingleton(options);
        services.AddSingleton<EodHDClient>();
        return services;
    }
}
namespace z019.BackgroundJobs;

using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBackgroundJobs(this IServiceCollection services)
    {
        return services
            .AddSingleton<UpdateExchangeTableService>()
            .AddTransient<UpdateExchangeTableJob>();
    }
}
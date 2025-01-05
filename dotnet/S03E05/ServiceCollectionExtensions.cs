namespace S03E05;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddS03E05(this IServiceCollection services)
    {
        services
            .AddCommonServices()
            .Configure<S03E05Options>(options =>
            {
                // Configure task-specific options here
            });

        return services;
    }
}

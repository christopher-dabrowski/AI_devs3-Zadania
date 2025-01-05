using Common.AiDevsApi.Extensions;
using S03E05.Models;
using Neo4j.Driver;

namespace S03E05;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddS03E05(this IServiceCollection services)
    {
        services.AddOptions<S03E05Options>()
            .BindConfiguration(S03E05Options.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services
            .AddAiDevsApi()
            .AddNeo4j();
    }

    private static IServiceCollection AddNeo4j(this IServiceCollection services) =>
        services.AddSingleton(sp =>
            {
                var taskOptions = sp.GetRequiredService<IOptions<S03E05Options>>().Value;

                var authToken = AuthTokens.Basic(taskOptions.Neo4jUser, taskOptions.Neo4jPassword);
                return GraphDatabase.Driver(taskOptions.Neo4jUri, authToken);
            });
}

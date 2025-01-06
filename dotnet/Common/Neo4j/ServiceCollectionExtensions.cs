using Common.OpenAi.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Neo4j.Driver;

namespace Common.OpenAi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNeo4j(this IServiceCollection services)
    {
        services.AddOptions<Neo4jOptions>()
            .BindConfiguration(Neo4jOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<Neo4jOptions>>().Value;
            var authToken = AuthTokens.Basic(options.User, options.Password);
            return GraphDatabase.Driver(options.Uri, authToken);
        });
    }
}

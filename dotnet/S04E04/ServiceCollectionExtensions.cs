using Common.OpenAI;

namespace S04E04;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddS04E04Services(this IServiceCollection services) => services
        .AddOpenAIChatClient();
}

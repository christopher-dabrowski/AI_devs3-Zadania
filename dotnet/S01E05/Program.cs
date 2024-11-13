using Common.AiDevsApi.Extensions;
using Microsoft.Extensions.DependencyInjection;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddAiDevsApi();

var host = builder.Build();

await using var scope = host.Services.CreateAsyncScope();
var serviceProvider = scope.ServiceProvider;

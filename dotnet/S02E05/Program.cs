using Common.AiDevsApi.Extensions;
using S02E05.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddAiDevsApi()
    .AddFirecrawl(builder.Configuration);

var host = builder.Build();
await using var scope = host.Services.CreateAsyncScope();
var sp = scope.ServiceProvider;

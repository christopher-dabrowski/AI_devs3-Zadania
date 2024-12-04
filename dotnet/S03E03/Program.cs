using Common.AiDevsApi.Extensions;
using Common.OpenAI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using S03E03;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddS03E03();

var host = builder.Build();
await using var scope = host.Services.CreateAsyncScope();
var sp = scope.ServiceProvider;

// Main logic will go here 

using System.Net.Http.Json;
using Common.AiDevsApi.Contracts;
using Common.AiDevsApi.Extensions;
using Common.AiDevsApi.Models;
using Common.Cache.Extensions;
using Common.OpenAI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Images;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddAiDevsApi()
    .AddOpenAIClient()
    .AddOpenAIChatClient()
    .AddCacheService();

var host = builder.Build();
await using var scope = host.Services.CreateAsyncScope();
var sp = scope.ServiceProvider;

using System.Collections.Immutable;
using Common.AiDevsApi.Contracts;
using Common.AiDevsApi.Models;
using Common.Cache.Contracts;
using OpenAI.Chat;
using S03E01;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddS03E01();

var host = builder.Build();
await using var scope = host.Services.CreateAsyncScope();
var sp = scope.ServiceProvider;


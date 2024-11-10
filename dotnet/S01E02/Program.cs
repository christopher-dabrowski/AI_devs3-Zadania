using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using S01E02.XyzApi.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddXyzApi();

var host = builder.Build();
await host.RunAsync();

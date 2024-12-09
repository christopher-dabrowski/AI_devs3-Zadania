using S03E04;
using S03E04.Models;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddS03E04();

var host = builder.Build();
await using var scope = host.Services.CreateAsyncScope();
var sp = scope.ServiceProvider;


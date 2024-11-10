using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenAI.Chat;

var builder = Host.CreateApplicationBuilder(args);

var host = builder.Build();

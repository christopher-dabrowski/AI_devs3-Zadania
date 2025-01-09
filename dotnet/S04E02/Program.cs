using S04E02;
using S04E02.Models;

var builder = Host.CreateApplicationBuilder(args);
builder.Services
    .AddS04E02Services();

var host = builder.Build();
var options = host.Services.GetRequiredService<IOptions<S04E02Options>>().Value;


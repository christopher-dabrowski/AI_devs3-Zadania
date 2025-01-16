using S04E04;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddS04E04Services();
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

await app.RunAsync();

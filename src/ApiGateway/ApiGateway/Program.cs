using Microsoft.AspNetCore.Server.Kestrel.Core;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", optional: false);
builder.Services.AddCors();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddOcelot();
builder.Services.AddOpenApi();
// Add services to the container.
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(7000, o => o.Protocols = HttpProtocols.Http1AndHttp2);
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors(policy =>
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader());

// Authentication BEFORE authorization (fix)
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok("Healthy"));
await app.UseOcelot();
app.Run();
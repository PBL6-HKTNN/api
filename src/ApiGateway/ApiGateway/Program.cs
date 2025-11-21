using Microsoft.AspNetCore.Server.Kestrel.Core;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", optional: false);
builder.Services.AddCors();
builder.Services.AddOcelot();
// Add services to the container.
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5006, o => o.Protocols = HttpProtocols.Http1AndHttp2);
    options.ListenAnyIP(7000, o => o.UseHttps().Protocols = HttpProtocols.Http1AndHttp2);
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseCors(policy =>
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader());
}

app.UseHttpsRedirection();

app.UseAuthorization();

await app.UseOcelot();
app.Run();

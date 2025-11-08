using Codemy.Notification.API.Services;
using Codemy.Notification.Application;
using Codemy.Notification.Application.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5180, o => o.Protocols = HttpProtocols.Http1AndHttp2);
    options.ListenAnyIP(7187, o => o.UseHttps().Protocols = HttpProtocols.Http1AndHttp2);
});
// Add services to the container. 
builder.Services.AddApplication();
builder.Services.AddControllers();
builder.Services.AddGrpc();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseCors("AllowAll");
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapGrpcService<NotificationServiceImpl>();

app.MapControllers();

app.Run();

using Codemy.FileStorage.Application.Interfaces;
using Codemy.FileStorage.Application.Services;
using Codemy.FileStorage.Infrastructure.Cloudinary;
using Codemy.FileStorage.Infrastructure.Configurations;
using Codemy.FileStorage.Infrastructure;
using Codemy.FileStorage.Application;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 1_000_000_000; // 1GB
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 1_000_000_000; // 1GB
});

builder.Services.Configure<KestrelServerOptions>(options => 
    options.Limits.MaxRequestBodySize = 200*1024*1024);

// Add services to the container.

// Add Cloudinary config
builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("CloudinarySettings"));

// Register services
builder.Services.AddScoped<IFileService, CloudinaryService>();
builder.Services.AddScoped<FileStorageAppService>();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddControllers();

// Add Swagger services

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
});

var app = builder.Build();

// Configure Swagger middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

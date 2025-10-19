using Codemy.FileStorage.Application.Interfaces;
using Codemy.FileStorage.Application.Services;
using Codemy.FileStorage.Infrastructure.Cloudinary;
using Codemy.FileStorage.Infrastructure.Configurations;
using Codemy.FileStorage.Infrastructure;
using Codemy.FileStorage.Application;

var builder = WebApplication.CreateBuilder(args);

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
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

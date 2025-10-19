using Codemy.FileStorage.Application.Interfaces;
using Codemy.FileStorage.Application.Services;
using Codemy.FileStorage.Infrastructure.Cloudinary;
using Codemy.FileStorage.Infrastructure.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add Cloudinary config
builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("CloudinarySettings"));

// Register services
builder.Services.AddScoped<IFileService, CloudinaryService>();
builder.Services.AddScoped<FileStorageAppService>();

builder.Services.AddControllers();
var app = builder.Build();

app.MapControllers();
app.Run();

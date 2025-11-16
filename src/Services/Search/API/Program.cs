using Codemy.Search.API.Services;
using Codemy.Search.Application;
using Codemy.Search.Infrastructure;
using Codemy.CoursesProto;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using DotNetEnv;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Clear default claim type mappings to keep original claim names (e.g., "nameid")
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

// ===== Load environment variables from .env file =====
Env.Load();

// ===== JWT Authentication =====
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
            ValidAudience = Environment.GetEnvironmentVariable("JWT_ISSUER"),
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET")!)
            ),
            ClockSkew = TimeSpan.Zero
        };

    });

// ===== Add Layer Services =====
builder.Services.AddInfrastructureServices();
builder.Services.AddApplicationServices();

builder.Services.AddGrpcClient<CoursesService.CoursesServiceClient>("CoursesGrpcClient", options =>
    {
        options.Address = new Uri("http://courses-service:5078");
    });

// ===== Background Service (Subscribe to Course Events) =====
builder.Services.AddHostedService<CourseEventSubscriber>();

// ===== HTTP & gRPC =====
builder.Services.AddControllers();
builder.Services.AddGrpc();
builder.Services.AddHttpContextAccessor();

// ===== CORS =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ===== OpenAPI / Swagger =====
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập token theo dạng: Bearer {your token}",
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    options.EnableAnnotations();
});

builder.Services.AddOpenApi();

// ===== Kestrel: HTTP + HTTPS + HTTP/2 (gRPC) =====
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5004, o => o.Protocols = HttpProtocols.Http1); // HTTP
    options.ListenAnyIP(5005, o => o.Protocols = HttpProtocols.Http2); // gRPC
    options.ListenAnyIP(7180, o => o.UseHttps().Protocols = HttpProtocols.Http1AndHttp2); // HTTPS + gRPC
});

var app = builder.Build();

// ===== Middleware Pipeline =====
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseCors("AllowAll");
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// ===== Map Endpoints =====
app.MapControllers();
app.MapGrpcService<CourseIndexGrpcService>(); // gRPC Service của Search

app.Run();
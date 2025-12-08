using Codemy.BuildingBlocks.Core.Models;
using Codemy.Identity.API.Services;
using Codemy.Identity.Application;
using Codemy.Identity.Infrastructure;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Clear default claim type mappings to keep original claim names (e.g., "nameid")
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

// Add authentication
Env.Load();
builder.Services.AddHttpContextAccessor();
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
                Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET"))
            ),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddInfrastructure(builder.Configuration);
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5197, o => o.Protocols = HttpProtocols.Http1);
    options.ListenAnyIP(5198, o => o.Protocols = HttpProtocols.Http2);
    options.ListenAnyIP(7046, o => o.UseHttps().Protocols = HttpProtocols.Http1AndHttp2);
});
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddGrpc();

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

});
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
}

app.UsePathBase("/api");
app.UseRouting();
app.UseAuthentication(); 
app.UseAuthorization(); 
app.MapGrpcService<IdentityGrpcService>();
app.UseMiddleware<PermissionMiddleware>();
app.MapControllers();
app.Run();

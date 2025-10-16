using Codemy.BuildingBlocks.Infrastructure;
using Codemy.Identity.Application;
using Codemy.Identity.Application.Services;
using Codemy.Identity.Infrastructure;
using Codemy.NotificationProto;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddInfrastructure(builder.Configuration);  // Infrastructure first
builder.Services.AddApplication();
builder.Services.AddControllers(); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); 
builder.Services.AddAuthorization();
var app = builder.Build(); 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
} 
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
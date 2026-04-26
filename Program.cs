

using Microsoft.EntityFrameworkCore;
using YAMBO.ShopService.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
var connectionString = builder.Configuration.GetConnectionString("YamboDatabase");
builder.Services.AddDbContext<YamboDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowUnity", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

var app = builder.Build();
app.UseCors("AllowUnity");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

Console.WriteLine("========================================");
Console.WriteLine("🎮 YAMBO Shop Service (C# ASP.NET Core)");
Console.WriteLine("========================================");
Console.WriteLine($"📍 URL: http://localhost:5000");
Console.WriteLine($"📖 Swagger: http://localhost:5000/swagger");
Console.WriteLine($"✅ Status: Running");
Console.WriteLine("========================================");

app.Run("http://localhost:5000");

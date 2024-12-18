using CityData.Core.Interface.Service;
using CityData.Services;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ICityService, CityService>();

builder.Services.AddMemoryCache(); // Add IMemoryCache service
builder.Services.UseHttpClientMetrics();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

#region Prometheus

app.UseMetricServer();
app.UseHttpMetrics();

#endregion Prometheus

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
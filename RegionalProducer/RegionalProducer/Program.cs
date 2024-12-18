using MassTransit;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Configuração de serviços
builder.Services.AddControllers(); // Adiciona os serviços de controladores
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.UseHttpClientMetrics();

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["Queue:Host"] ?? "localhost", "/", h =>
        {
            h.Username(builder.Configuration["Queue:User"] ?? "guest");
            h.Password(builder.Configuration["Queue:Password"] ?? "guest");
        });
    });
});

var app = builder.Build();

#region Prometheus

app.UseMetricServer();
app.UseHttpMetrics();

#endregion Prometheus

// Middleware e pipeline

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

// Mapear controladores
app.MapControllers();

app.Run();
var builder = WebApplication.CreateBuilder(args);

// Configuração de serviços
builder.Services.AddControllers(); // Adiciona os serviços de controladores
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware e pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Mapear controladores
app.MapControllers();

app.Run();
using ApiCatalogo_Galactico.Data;
using ApiCatalogo_Galactico.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Falta la cadena de conexión DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 0, 36)),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure()));

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddScoped<IPersonajeService, PersonajeService>();
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddScoped<IEventoService, EventoService>();
builder.Services.AddScoped<IEstadisticaService, EstadisticaService>();
builder.Services.AddScoped<ISimulacionService, SimulacionService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
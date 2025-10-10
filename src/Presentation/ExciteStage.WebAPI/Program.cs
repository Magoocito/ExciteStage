using ExciteStage.Infrastructure.Persistance.Service;

var builder = WebApplication.CreateBuilder(args);

// Registrar servicios de persistencia (incluye AutoMapper)
builder.Services.ConfigurePersistenceApp(builder.Configuration);

// Registrar servicios de aplicación (si tienes mapeos/handlers en Application)
//builder.Services.ConfigureApplicationApp();
//builder.Services.ConfigureCorsPolicy();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

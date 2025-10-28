using ExciteStage.Infrastructure.ML;
using ExciteStage.Infrastructure.ML.Features;
using ExciteStage.Infrastructure.Persistance.Service;
using Microsoft.Extensions.ML;

var builder = WebApplication.CreateBuilder(args);

// Registrar servicios de persistencia (incluye AutoMapper)
builder.Services.ConfigurePersistenceApp(builder.Configuration);

// Registrar servicios de aplicación (si tienes mapeos/handlers en Application)
//builder.Services.ConfigureApplicationApp();
//builder.Services.ConfigureCorsPolicy();

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddPredictionEnginePool<MatchFeatures, MatchPrediction>()
    .FromFile(modelName: "MatchModel", filePath: "Data/Models/match_predictor.zip", watchForChanges: true);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

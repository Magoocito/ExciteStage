using Microsoft.ML;

namespace ExciteStage.Infrastructure.ML
{
    public class MLNetTrainer
    {
        private readonly string _csvPath = "Data/liga1_historical.csv";
        private readonly string _modelPath = "Data/Models/match_predictor.zip";

        public void TrainInitialModel()
        {
            var mlContext = new MLContext(seed: 42);

            // 1. Cargar datos
            var dataView = mlContext.Data.LoadFromTextFile<MatchFeatures>(
                _csvPath,
                separatorChar: ',',
                hasHeader: true
            );

            // 2. Pipeline de entrenamiento
            var pipeline = mlContext.Transforms.Concatenate("Features",
                    nameof(MatchFeatures.HomeAltitude),
                    nameof(MatchFeatures.TravelDistance),
                    nameof(MatchFeatures.IsHighAltitude))
                .Append(mlContext.Regression.Trainers.FastTree(
                    labelColumnName: "Label",
                    featureColumnName: "Features"
                ));

            // 3. Entrenar el modelo
            var model = pipeline.Fit(dataView);

            // 4. Guardar el modelo
            Directory.CreateDirectory(Path.GetDirectoryName(_modelPath)!);
            mlContext.Model.Save(model, dataView.Schema, _modelPath);

            // 5. Evaluar el modelo
            var predictions = model.Transform(dataView);
            var metrics = mlContext.Regression.Evaluate(predictions, labelColumnName: "Label");

            Console.WriteLine($"Entrenamiento completado.");
            Console.WriteLine($"R²: {metrics.RSquared:F3}");
            Console.WriteLine($"MAE: {metrics.MeanAbsoluteError:F3}");
            Console.WriteLine($"RMSE: {metrics.RootMeanSquaredError:F3}");
        }
    }
}
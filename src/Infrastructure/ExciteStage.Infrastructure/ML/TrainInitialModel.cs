using ExciteStage.Infrastructure.ML.Features;
using Microsoft.ML;

namespace ExciteStage.Infrastructure.ML
{
    public class MLNetTrainer
    {
        private readonly string _csvPath = "Data/liga1_2025_sample.csv";
        private readonly string _modelPath = "Data/Models/match_predictor.zip";

        public void TrainInitialModel()
        {
            var mlContext = new MLContext(seed: 42);

            // 1. Cargar datos de entrenamiento (incluye resultado)
            var dataView = mlContext.Data.LoadFromTextFile<MatchTrainingData>(
                _csvPath,
                separatorChar: ',',
                hasHeader: true
            );

            // 2. Pipeline de entrenamiento para REGRESIÓN (predice probabilidades)
            var pipeline = mlContext.Transforms.Concatenate("Features",
                    nameof(MatchFeatures.HomeAltitude),
                    nameof(MatchFeatures.TravelDistance),
                    nameof(MatchFeatures.IsHighAltitude),
                    nameof(MatchFeatures.RefereeBias),
                    nameof(MatchFeatures.WeatherImpactEncoded),
                    nameof(MatchFeatures.HomeFormLast5),
                    nameof(MatchFeatures.AwayFormLast5),
                    nameof(MatchFeatures.HeadToHeadWins),
                    nameof(MatchFeatures.PPG),
                    nameof(MatchFeatures.CS),
                    nameof(MatchFeatures.BTTS),
                    nameof(MatchFeatures.xGF)
                )
                .Append(mlContext.Regression.Trainers.FastTree(
                    labelColumnName: nameof(MatchTrainingData.HomeWin), // Usa HomeWin como label
                    featureColumnName: "Features"
                ));

            // 3. Dividir los datos en conjuntos de entrenamiento y prueba
            var split = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);

            // 4. Entrenar el modelo
            var model = pipeline.Fit(split.TrainSet);

            // 5. Hacer predicciones y evaluar el modelo
            var predictions = model.Transform(split.TestSet);
            var metrics = mlContext.Regression.Evaluate(predictions, labelColumnName: nameof(MatchTrainingData.HomeWin));

            // 6. Guardar el modelo
            Directory.CreateDirectory(Path.GetDirectoryName(_modelPath)!);
            mlContext.Model.Save(model, dataView.Schema, _modelPath);

            Console.WriteLine($"✅ Entrenamiento completado.");
            Console.WriteLine($"📊 R²: {metrics.RSquared:F3}");
            Console.WriteLine($"📊 MAE: {metrics.MeanAbsoluteError:F3}");
            Console.WriteLine($"📊 RMSE: {metrics.RootMeanSquaredError:F3}");
            Console.WriteLine($"💾 Modelo guardado en: {_modelPath}");
        }

        /// <summary>
        /// Entrena un modelo específico para Over/Under 2.5
        /// </summary>
        public void TrainOverUnderModel()
        {
            var mlContext = new MLContext(seed: 42);

            // Cargar datos y crear feature Over25 (1 si >2.5 goles, 0 si no)
            var dataView = mlContext.Data.LoadFromTextFile<MatchTrainingData>(_csvPath, separatorChar: ',', hasHeader: true);
            
            var pipeline = mlContext.Transforms.Expression("Over25", "HomeGoals + AwayGoals > 2.5 ? 1.0f : 0.0f", "HomeGoals", "AwayGoals")
                .Append(mlContext.Transforms.Concatenate("Features",
                    nameof(MatchFeatures.HomeAltitude),
                    nameof(MatchFeatures.TravelDistance),
                    nameof(MatchFeatures.IsHighAltitude),
                    nameof(MatchFeatures.HomeFormLast5),
                    nameof(MatchFeatures.AwayFormLast5)
                ))
                .Append(mlContext.BinaryClassification.Trainers.FastTree(
                    labelColumnName: "Over25",
                    featureColumnName: "Features"
                ));

            var model = pipeline.Fit(dataView);
            var overUnderModelPath = "Data/Models/over_under_predictor.zip";
            Directory.CreateDirectory(Path.GetDirectoryName(overUnderModelPath)!);
            mlContext.Model.Save(model, dataView.Schema, overUnderModelPath);
            
            Console.WriteLine($"✅ Modelo Over/Under entrenado y guardado en: {overUnderModelPath}");
        }
    }
}
using Microsoft.ML.Data;

namespace ExciteStage.Infrastructure.ML.Features
{
    /// <summary>
    /// Clase de características (features) para el modelo ML.NET.
    /// Contiene SOLO las features de entrada, NO el resultado.
    /// </summary>
    public class MatchFeatures
    {
        // ========================================
        // Características básicas del partido
        // ========================================

        [LoadColumn(0)]
        public float HomeAltitude { get; set; }

        [LoadColumn(1)]
        public float TravelDistance { get; set; }

        [LoadColumn(2)]
        public float IsHighAltitude { get; set; } // 0 o 1

        [LoadColumn(3)]
        public float RefereeBias { get; set; } // 0-10

        [LoadColumn(4)]
        public float WeatherImpactEncoded { get; set; } // 0-1

        // ========================================
        // Características adicionales
        // ========================================

        [LoadColumn(5)]
        public float HomeFormLast5 { get; set; } // Puntos últimos 5 partidos

        [LoadColumn(6)]
        public float AwayFormLast5 { get; set; }

        [LoadColumn(7)]
        public float HeadToHeadWins { get; set; } // Victorias históricas home vs away

        [LoadColumn(8)]
        public float PPG { get; set; } // Points per game

        [LoadColumn(9)]
        public float CS { get; set; } // Clean sheets

        [LoadColumn(10)]
        public float BTTS { get; set; } // Both teams to score

        [LoadColumn(11)]
        public float xGF { get; set; } // Expected goals for
    }

    /// <summary>
    /// Clase para datos de entrenamiento que incluye el resultado.
    /// Se usa SOLO durante el entrenamiento.
    /// </summary>
    public class MatchTrainingData : MatchFeatures
    {
        [LoadColumn(12)]
        public float HomeWin { get; set; } // 0, 0.5, 1 (resultado real)

        [LoadColumn(13)]
        public float HomeGoals { get; set; }

        [LoadColumn(14)]
        public float AwayGoals { get; set; }
    }
}
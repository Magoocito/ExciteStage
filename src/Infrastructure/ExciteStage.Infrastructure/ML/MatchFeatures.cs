using Microsoft.ML.Data;

namespace ExciteStage.Infrastructure.ML.Features
{
    /// <summary>
    /// Clase de características (features) para el modelo ML.NET.
    /// Debe coincidir con las columnas del CSV de entrenamiento.
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
        // Características adicionales (opcionales)
        // ========================================

        [LoadColumn(5)]
        public float HomeFormLast5 { get; set; } // Puntos últimos 5 partidos

        [LoadColumn(6)]
        public float AwayFormLast5 { get; set; }

        [LoadColumn(7)]
        public float HeadToHeadWins { get; set; } // Victorias históricas home vs away

        // NOTA: Si agregas más columnas, actualiza los índices [LoadColumn]
        // y asegúrate de que el CSV de entrenamiento las incluya
    }
}
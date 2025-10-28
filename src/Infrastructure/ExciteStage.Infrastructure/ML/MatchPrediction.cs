using Microsoft.ML.Data;

namespace ExciteStage.Infrastructure.ML
{
    /// <summary>
    /// Predicción del modelo ML para apuestas.
    /// Contiene probabilidades calculadas por el modelo.
    /// </summary>
    public class MatchPrediction
    {
        [ColumnName("Score")]
        public float HomeWinProbability { get; set; }

        public float DrawProbability { get; set; }
        public float AwayWinProbability { get; set; }
        public float Over25Probability { get; set; }
        public float BothScoreProbability { get; set; }
        public float Confidence { get; set; }

        /// <summary>
        /// Calcula las probabilidades normalizadas para 1X2
        /// </summary>
        public void NormalizeProbabilities()
        {
            var total = HomeWinProbability + DrawProbability + AwayWinProbability;
            if (total > 0)
            {
                HomeWinProbability /= total;
                DrawProbability /= total;
                AwayWinProbability /= total;
            }
        }

        /// <summary>
        /// Calcula las probabilidades para Over/Under 2.5
        /// </summary>
        public void CalculateOverUnderProbabilities()
        {
            // Heurística simple: más goles en partidos de altura
            Over25Probability = Math.Min(HomeWinProbability + DrawProbability * 0.3f, 0.8f);
        }

        /// <summary>
        /// Calcula probabilidad de Both Teams to Score
        /// </summary>
        public void CalculateBTTSProbability()
        {
            // Heurística: BTTS más probable en partidos equilibrados
            var balance = Math.Abs(HomeWinProbability - AwayWinProbability);
            BothScoreProbability = Math.Max(0.3f, 0.7f - balance);
        }
    }
}

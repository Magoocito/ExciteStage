using ExciteStage.Domain.Entities;
using ExciteStage.Domain.ValueObjects;

namespace ExciteStage.Application.Services
{   
    public class MLPredictionService : IMLPredictionService
    {
        public async Task<MatchPredictions> GeneratePredictionsAsync(Match match)
        {
            // Simulación de integración ML.NET o servicio Python externo
            var altitudeFactor = new AltitudeFactor(match.Altitude, 0);
            var travelBurden = new TravelBurden(match.TravelDistance);

            var baseProbabilities = new
            {
                HomeWin = 0.45,
                Draw = 0.27,
                AwayWin = 0.28,
                Over25 = 0.55,
                BothScore = 0.60
            };

            var homeWinAdj = baseProbabilities.HomeWin + altitudeFactor.ImpactFactor - travelBurden.FatigueImpact;

            return new MatchPredictions
            {
                HomeWinProbability = Math.Max(0.1, Math.Min(0.9, homeWinAdj)),
                DrawProbability = baseProbabilities.Draw,
                AwayWinProbability = 1 - homeWinAdj - baseProbabilities.Draw,
                Over25Probability = baseProbabilities.Over25,
                BothScoreProbability = baseProbabilities.BothScore,
                Confidence = 0.75 + (altitudeFactor.IsSignificant() ? 0.1 : 0.0)
            };
        }

        public async Task<ExpertSentiment> GetExpertOpinionAsync(Match match)
        {
            // TODO: Integrar scraping de Telegram/Twitter
            return new ExpertSentiment
            {
                OverallSentiment = 0.0,
                ExpertCount = 0,
                Confidence = 0.5
            };
        }
    }
}

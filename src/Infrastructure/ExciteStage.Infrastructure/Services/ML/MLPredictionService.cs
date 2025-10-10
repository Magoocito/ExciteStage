using AutoMapper;
using ExciteStage.Application.Services;
using ExciteStage.Domain.Entities;
using ExciteStage.Domain.ValueObjects;
using ExciteStage.Infrastructure.ML;
using ExciteStage.Infrastructure.ML.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ML;

namespace ExciteStage.Infrastructure.Services.ML
{
    /// <summary>
    /// Servicio de predicción ML usando PredictionEnginePool (thread-safe y optimizado).
    /// RECOMENDADO para producción en ASP.NET Core.
    /// </summary>
    public class MLPredictionService : IMLPredictionService
    {
        private readonly PredictionEnginePool<MatchFeatures, MatchPrediction> _predictionEnginePool;
        private readonly IMapper _mapper;
        private readonly ILogger<MLPredictionService> _logger;

        // ✅ Inyecta el pool en lugar del engine individual
        public MLPredictionService(
            PredictionEnginePool<MatchFeatures, MatchPrediction> predictionEnginePool,
            IMapper mapper,
            ILogger<MLPredictionService> logger)
        {
            _predictionEnginePool = predictionEnginePool ?? throw new ArgumentNullException(nameof(predictionEnginePool));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<MatchPredictions> GeneratePredictionsAsync(Match match)
        {
            try
            {
                _logger.LogInformation("Generating predictions for match: {HomeTeam} vs {AwayTeam}",
                    match.HomeTeam, match.AwayTeam);

                // 1. Mapear Match → MatchFeatures
                var features = _mapper.Map<MatchFeatures>(match);

                // 2. Ejecutar predicción usando el pool (thread-safe)
                var rawPrediction = _predictionEnginePool.Predict(features);

                // 3. Aplicar ajustes contextuales peruanos
                var adjustedPrediction = ApplyContextualAdjustments(rawPrediction, match);

                // 4. Mapear MatchPrediction → MatchPredictions (Value Object)
                var result = _mapper.Map<MatchPredictions>(adjustedPrediction);

                _logger.LogInformation(
                    "Prediction completed: HomeWin={HomeWin:P2}, Draw={Draw:P2}, AwayWin={AwayWin:P2}, Confidence={Confidence:P2}",
                    result.HomeWinProbability, result.DrawProbability, result.AwayWinProbability, result.Confidence);

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating predictions for match {MatchId}", match.Id);
                throw;
            }
        }

        public async Task<ExpertSentiment> GetExpertOpinionAsync(Match match)
        {
            try
            {
                _logger.LogInformation("Calculating expert sentiment for match: {HomeTeam} vs {AwayTeam}",
                    match.HomeTeam, match.AwayTeam);

                // TODO: Integrar con Twitter API, Telegram scraping
                var sentiment = CalculateBasicSentiment(match);

                return await Task.FromResult(sentiment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating sentiment for match {MatchId}", match.Id);

                return new ExpertSentiment
                {
                    OverallSentiment = 0.5,
                    ExpertCount = 0,
                    Confidence = 0.3
                };
            }
        }

        /// <summary>
        /// Aplica ajustes contextuales peruanos (altitud, viajes, etc.).
        /// </summary>
        private MatchPrediction ApplyContextualAdjustments(MatchPrediction rawPrediction, Match match)
        {
            // 1. Factor de altitud
            var altitudeFactor = new AltitudeFactor(match.Altitude, 0);
            var altitudeBoost = altitudeFactor.IsSignificant() ? altitudeFactor.ImpactFactor : 0.0;

            // 2. Factor de viaje (fatiga)
            var travelBurden = new TravelBurden(match.TravelDistance);
            var travelPenalty = travelBurden.IsHighImpact() ? travelBurden.FatigueImpact : 0.0;

            // 3. Ajustar probabilidades
            var adjustedHomeWin = Math.Min(
                rawPrediction.HomeWinProbability * (1 + altitudeBoost + travelPenalty),
                0.95);

            var adjustedAwayWin = Math.Max(
                rawPrediction.AwayWinProbability * (1 - travelPenalty),
                0.05);

            var adjustedDraw = 1.0 - adjustedHomeWin - adjustedAwayWin;

            // 4. Normalizar para asegurar que sumen 1.0
            var total = adjustedHomeWin + adjustedDraw + adjustedAwayWin;
            adjustedHomeWin /= total;
            adjustedDraw /= total;
            adjustedAwayWin /= total;

            _logger.LogDebug(
                "Contextual adjustments applied: AltitudeBoost={AltitudeBoost:P2}, TravelPenalty={TravelPenalty:P2}",
                altitudeBoost, travelPenalty);

            return new MatchPrediction
            {
                HomeWinProbability = (float)adjustedHomeWin,
                DrawProbability = (float)adjustedDraw,
                AwayWinProbability = (float)adjustedAwayWin,
                Over25Probability = rawPrediction.Over25Probability,
                BothScoreProbability = rawPrediction.BothScoreProbability,
                Confidence = (float)(rawPrediction.Confidence * 0.95) // Reducir confianza tras ajustes
            };
        }

        /// <summary>
        /// Calcula sentiment básico basado en heurísticas simples.
        /// </summary>
        private ExpertSentiment CalculateBasicSentiment(Match match)
        {
            // Heurística: partidos de altura favorecen al local
            var baseSentiment = match.IsHighAltitude() ? 0.65 : 0.50;

            // Ajustar por distancia de viaje
            var travelBurden = new TravelBurden(match.TravelDistance);
            var sentimentAdjustment = travelBurden.IsHighImpact() ? 0.10 : 0.0;

            var finalSentiment = Math.Min(baseSentiment + sentimentAdjustment, 0.85);

            return new ExpertSentiment
            {
                OverallSentiment = finalSentiment,
                ExpertCount = 5, // Simulado
                Confidence = 0.6
            };
        }
    }
}
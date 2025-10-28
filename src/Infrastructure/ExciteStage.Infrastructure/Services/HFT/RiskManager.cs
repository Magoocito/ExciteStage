using ExciteStage.Application.Services.HFT;
using ExciteStage.Domain.Entities;
using ExciteStage.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace ExciteStage.Infrastructure.Services.HFT
{
    /// <summary>
    /// Gestor de riesgo implementando Kelly Criterion y principios HFT
    /// </summary>
    public class RiskManager : IRiskManager
    {
        private readonly ILogger<RiskManager> _logger;
        private const decimal MAX_POSITION_SIZE = 0.15m; // 15% máximo por posición
        private const decimal MAX_PORTFOLIO_EXPOSURE = 0.15m; // 15% máximo total
        private const decimal STOP_LOSS_THRESHOLD = 0.05m; // 5% stop-loss
        private const decimal KELLY_FRACTION_LIMIT = 0.25m; // Máximo 25% Kelly

        public RiskManager(ILogger<RiskManager> logger)
        {
            _logger = logger;
        }

        public async Task<decimal> CalculateOptimalPositionSizeAsync(
            decimal probability, 
            decimal odds, 
            decimal bankroll, 
            decimal currentExposure)
        {
            try
            {
                // Kelly Criterion: f = (bp - q) / b
                // b = odds - 1 (ganancia neta)
                // p = probabilidad de ganar
                // q = probabilidad de perder (1-p)
                
                var b = odds - 1; // Ganancia neta
                var p = probability; // Probabilidad de ganar
                var q = 1 - p; // Probabilidad de perder

                // Kelly fraction
                var kellyFraction = (b * p - q) / b;

                // Aplicar límites de seguridad
                kellyFraction = Math.Max(0, Math.Min(kellyFraction, KELLY_FRACTION_LIMIT));

                // Calcular tamaño de posición
                var positionSize = kellyFraction * bankroll;

                // Verificar límites de exposición
                var maxAllowedPosition = (MAX_PORTFOLIO_EXPOSURE - currentExposure) * bankroll;
                positionSize = Math.Min(positionSize, maxAllowedPosition);

                // Verificar límite individual
                positionSize = Math.Min(positionSize, MAX_POSITION_SIZE * bankroll);

                _logger.LogDebug(
                    "Position sizing: Kelly={Kelly:P2}, Position={Position:C}, Exposure={Exposure:P2}",
                    kellyFraction, positionSize, currentExposure);

                return await Task.FromResult(Math.Max(0, positionSize));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating position size");
                return 0;
            }
        }

        public async Task<bool> ValidatePositionAsync(PortfolioBet bet, BettingPortfolio portfolio)
        {
            try
            {
                // Verificar límite de stake individual
                if (bet.StakePercent > MAX_POSITION_SIZE)
                {
                    _logger.LogWarning("Bet stake {Stake:P2} exceeds individual limit {Limit:P2}",
                        bet.StakePercent, MAX_POSITION_SIZE);
                    return false;
                }

                // Verificar exposición total del portafolio
                var totalExposure = portfolio.TotalStakePercent + bet.StakePercent;
                if (totalExposure > MAX_PORTFOLIO_EXPOSURE)
                {
                    _logger.LogWarning("Portfolio exposure {Exposure:P2} would exceed limit {Limit:P2}",
                        totalExposure, MAX_PORTFOLIO_EXPOSURE);
                    return false;
                }

                // Verificar correlación con apuestas existentes
                var correlationRisk = await CalculateCorrelationRiskAsync(bet, portfolio);
                if (correlationRisk > 0.8m) // 80% correlación máxima
                {
                    _logger.LogWarning("High correlation risk {Risk:P2} for bet type {BetType}",
                        correlationRisk, bet.BetType);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating position");
                return false;
            }
        }

        public async Task<decimal> CalculatePortfolioRiskAsync(BettingPortfolio portfolio)
        {
            try
            {
                // Calcular riesgo basado en exposición y correlación
                var exposureRisk = portfolio.TotalStakePercent / MAX_PORTFOLIO_EXPOSURE;
                
                var correlationRisk = await CalculatePortfolioCorrelationAsync(portfolio);
                
                // Riesgo total = exposición + correlación
                var totalRisk = (exposureRisk + correlationRisk) / 2;

                _logger.LogDebug("Portfolio risk: Exposure={Exposure:P2}, Correlation={Correlation:P2}, Total={Total:P2}",
                    exposureRisk, correlationRisk, totalRisk);

                return await Task.FromResult(Math.Min(1, totalRisk));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating portfolio risk");
                return 1; // Máximo riesgo en caso de error
            }
        }

        public async Task<bool> ShouldTriggerStopLossAsync(BettingPortfolio portfolio, decimal currentValue)
        {
            try
            {
                var initialValue = portfolio.Bets.Sum(b => b.StakeAmount);
                var drawdown = (initialValue - currentValue) / initialValue;

                var shouldStop = drawdown >= STOP_LOSS_THRESHOLD;

                if (shouldStop)
                {
                    _logger.LogWarning("Stop-loss triggered: Drawdown {Drawdown:P2} >= Threshold {Threshold:P2}",
                        drawdown, STOP_LOSS_THRESHOLD);
                }

                return await Task.FromResult(shouldStop);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking stop-loss");
                return false;
            }
        }

        public async Task<decimal> CalculateMaxDrawdownAsync(BettingPortfolio portfolio)
        {
            try
            {
                // Calcular drawdown máximo basado en Kelly y exposición
                var kellyRisk = portfolio.Bets.Average(b => b.StakePercent) * 2; // Kelly * 2 como buffer
                var exposureRisk = portfolio.TotalStakePercent;
                
                var maxDrawdown = Math.Max(kellyRisk, exposureRisk) * 0.5m; // 50% del riesgo como drawdown máximo

                return await Task.FromResult(Math.Min(maxDrawdown, 0.1m)); // Máximo 10%
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating max drawdown");
                return 0.1m; // 10% por defecto
            }
        }

        private async Task<decimal> CalculateCorrelationRiskAsync(PortfolioBet newBet, BettingPortfolio portfolio)
        {
            // Calcular correlación entre la nueva apuesta y las existentes
            var correlationSum = 0m;
            var betCount = 0;

            foreach (var existingBet in portfolio.Bets)
            {
                var correlation = CalculateBetCorrelation(newBet.BetType, existingBet.BetType);
                correlationSum += correlation;
                betCount++;
            }

            return await Task.FromResult(betCount > 0 ? correlationSum / betCount : 0);
        }

        private async Task<decimal> CalculatePortfolioCorrelationAsync(BettingPortfolio portfolio)
        {
            if (portfolio.Bets.Count <= 1) return 0;

            var totalCorrelation = 0m;
            var pairCount = 0;

            for (int i = 0; i < portfolio.Bets.Count; i++)
            {
                for (int j = i + 1; j < portfolio.Bets.Count; j++)
                {
                    var correlation = CalculateBetCorrelation(
                        portfolio.Bets[i].BetType, 
                        portfolio.Bets[j].BetType);
                    totalCorrelation += correlation;
                    pairCount++;
                }
            }

            return await Task.FromResult(pairCount > 0 ? totalCorrelation / pairCount : 0);
        }

        private decimal CalculateBetCorrelation(BetType bet1, BetType bet2)
        {
            // Matriz de correlación entre tipos de apuestas
            return (bet1, bet2) switch
            {
                // Apuestas mutuamente excluyentes (correlación negativa)
                (BetType.HomeWin, BetType.AwayWin) => -0.8m,
                (BetType.Over25, BetType.Under25) => -1.0m,
                (BetType.BothTeamsToScore, BetType.BothTeamsNotToScore) => -1.0m,
                
                // Apuestas correlacionadas positivamente
                (BetType.HomeWin, BetType.Over25) => 0.3m,
                (BetType.AwayWin, BetType.Over25) => 0.3m,
                (BetType.BothTeamsToScore, BetType.Over25) => 0.6m,
                
                // Apuestas independientes
                _ => 0.1m
            };
        }
    }
}

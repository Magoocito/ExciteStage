using ExciteStage.Application.Services.HFT;
using ExciteStage.Domain.Entities;
using ExciteStage.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace ExciteStage.Application.Services
{
    /// <summary>
    /// Optimizador de portafolio estilo HFT para apuestas de fútbol peruano
    /// </summary>
    public class PortfolioOptimizer : IPortfolioOptimizer
    {
        private readonly IRiskManager _riskManager;
        private readonly IArbitrageEngine _arbitrageEngine;
        private readonly ILogger<PortfolioOptimizer> _logger;

        public PortfolioOptimizer(
            IRiskManager riskManager,
            IArbitrageEngine arbitrageEngine,
            ILogger<PortfolioOptimizer> logger)
        {
            _riskManager = riskManager;
            _arbitrageEngine = arbitrageEngine;
            _logger = logger;
        }

        public async Task<BettingPortfolio> OptimizePortfolioAsync(
            Match match,
            MatchPredictions predictions,
            List<MarketOdds> availableOdds,
            decimal maxRiskPercent = 0.15m)
        {
            try
            {
                _logger.LogInformation("Optimizing portfolio for match: {HomeTeam} vs {AwayTeam}",
                    match.HomeTeam, match.AwayTeam);

                var portfolio = new BettingPortfolio(match.Id);

                // 1. Buscar oportunidades de arbitraje
                var arbitrageOpportunities = await _arbitrageEngine.FindArbitrageOpportunitiesAsync(match);
                if (arbitrageOpportunities.Any())
                {
                    await AddArbitrageBetsAsync(portfolio, arbitrageOpportunities, maxRiskPercent);
                }

                // 2. Buscar apuestas con valor positivo
                var valueBets = await _arbitrageEngine.FindValueBetsAsync(match, predictions, availableOdds);
                await AddValueBetsAsync(portfolio, valueBets, maxRiskPercent);

                // 3. Aplicar diversificación
                await ApplyDiversificationAsync(portfolio, match, predictions, availableOdds);

                // 4. Validar y ajustar riesgo
                await ValidateAndAdjustRiskAsync(portfolio, maxRiskPercent);

                _logger.LogInformation("Portfolio optimized: {BetCount} bets, {TotalStake:P2} exposure",
                    portfolio.Bets.Count, portfolio.TotalStakePercent);

                return portfolio;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error optimizing portfolio for match {MatchId}", match.Id);
                return new BettingPortfolio(match.Id);
            }
        }

        public async Task<BettingPortfolio> RebalancePortfolioAsync(
            BettingPortfolio currentPortfolio,
            List<MarketOdds> updatedOdds,
            MatchPredictions updatedPredictions)
        {
            try
            {
                _logger.LogInformation("Rebalancing portfolio with {OddsCount} updated odds",
                    updatedOdds.Count);

                // 1. Evaluar posiciones existentes
                var positionsToClose = new List<PortfolioBet>();
                var positionsToAdjust = new List<PortfolioBet>();

                foreach (var bet in currentPortfolio.Bets)
                {
                    var updatedOddsForBet = updatedOdds.FirstOrDefault(o => o.BetType == bet.BetType);
                    if (updatedOddsForBet != null)
                    {
                        var newProbability = GetProbabilityForBetType(bet.BetType, updatedPredictions);
                        var newEV = await _arbitrageEngine.CalculateExpectedValueAsync(
                            newProbability, updatedOddsForBet.Odds, bet.StakeAmount);

                        if (newEV < 0) // EV negativo, cerrar posición
                        {
                            positionsToClose.Add(bet);
                        }
                        else if (Math.Abs(newEV - (decimal)bet.ExpectedReturn) > (decimal)bet.ExpectedReturn * 0.2m) // 20% cambio
                        {
                            positionsToAdjust.Add(bet);
                        }
                    }
                }

                // 2. Cerrar posiciones con EV negativo
                foreach (var bet in positionsToClose)
                {
                    currentPortfolio.Bets.Remove(bet);
                    _logger.LogInformation("Closed position: {BetType} (EV: {EV:C})",
                        bet.BetType, bet.ExpectedReturn);
                }

                // 3. Ajustar posiciones con cambios significativos
                foreach (var bet in positionsToAdjust)
                {
                    var updatedOddsForBet = updatedOdds.FirstOrDefault(o => o.BetType == bet.BetType);
                    if (updatedOddsForBet != null)
                    {
                        var newProbability = GetProbabilityForBetType(bet.BetType, updatedPredictions);
                        var newStake = await _riskManager.CalculateOptimalPositionSizeAsync(
                            newProbability, updatedOddsForBet.Odds, 10000, currentPortfolio.TotalStakePercent);

                        bet.StakeAmount = newStake;
                        bet.StakePercent = newStake / 10000; // Asumiendo bankroll de 10k
                        bet.ExpectedReturn = await _arbitrageEngine.CalculateExpectedValueAsync(
                            newProbability, updatedOddsForBet.Odds, newStake);

                        _logger.LogInformation("Adjusted position: {BetType} (New Stake: {Stake:C})",
                            bet.BetType, newStake);
                    }
                }

                return currentPortfolio;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rebalancing portfolio");
                return currentPortfolio;
            }
        }

        public async Task<PortfolioDiversification> CalculateOptimalDiversificationAsync(
            Match match,
            MatchPredictions predictions,
            List<MarketOdds> availableOdds)
        {
            try
            {
                var diversification = new PortfolioDiversification();

                // Calcular asignación óptima basada en EV y correlación
                var valueBets = await _arbitrageEngine.FindValueBetsAsync(match, predictions, availableOdds);
                
                var totalEV = valueBets.Sum(vb => vb.ExpectedValue);
                if (totalEV > 0)
                {
                    diversification.HomeWinAllocation = valueBets
                        .Where(vb => vb.BetType == BetType.HomeWin)
                        .Sum(vb => vb.ExpectedValue) / totalEV;

                    diversification.DrawAllocation = valueBets
                        .Where(vb => vb.BetType == BetType.Draw)
                        .Sum(vb => vb.ExpectedValue) / totalEV;

                    diversification.AwayWinAllocation = valueBets
                        .Where(vb => vb.BetType == BetType.AwayWin)
                        .Sum(vb => vb.ExpectedValue) / totalEV;

                    diversification.OverUnderAllocation = valueBets
                        .Where(vb => vb.BetType == BetType.Over25 || vb.BetType == BetType.Under25)
                        .Sum(vb => vb.ExpectedValue) / totalEV;

                    diversification.BTTSAllocation = valueBets
                        .Where(vb => vb.BetType == BetType.BothTeamsToScore || vb.BetType == BetType.BothTeamsNotToScore)
                        .Sum(vb => vb.ExpectedValue) / totalEV;
                }

                diversification.TotalAllocation = diversification.HomeWinAllocation + 
                                                diversification.DrawAllocation + 
                                                diversification.AwayWinAllocation + 
                                                diversification.OverUnderAllocation + 
                                                diversification.BTTSAllocation;

                // Calcular score de riesgo basado en correlación
                diversification.RiskScore = await CalculateDiversificationRiskAsync(valueBets, match);

                // Recomendar apuestas específicas
                diversification.RecommendedBets = valueBets
                    .OrderByDescending(vb => vb.ExpectedValue)
                    .Take(5)
                    .Select(vb => $"{vb.BetType} @ {vb.MarketOdds:F2} (EV: {vb.ExpectedValue:C})")
                    .ToList();

                return diversification;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating diversification");
                return new PortfolioDiversification();
            }
        }

        public async Task<PortfolioAction> ExecuteStopLossAsync(BettingPortfolio portfolio)
        {
            try
            {
                var currentValue = portfolio.Bets.Sum(b => b.ExpectedReturn);
                var shouldStop = await _riskManager.ShouldTriggerStopLossAsync(portfolio, currentValue);

                if (shouldStop)
                {
                    var action = new PortfolioAction
                    {
                        Type = ActionType.StopLoss,
                        Reason = "Portfolio drawdown exceeded threshold",
                        Amount = currentValue,
                        BetsToClose = portfolio.Bets.ToList()
                    };

                    _logger.LogWarning("Stop-loss executed: {Reason}, Amount: {Amount:C}",
                        action.Reason, action.Amount);

                    return action;
                }

                return new PortfolioAction { Type = ActionType.Hold };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing stop-loss");
                return new PortfolioAction { Type = ActionType.Hold };
            }
        }

        public async Task<PortfolioMetrics> CalculatePortfolioMetricsAsync(BettingPortfolio portfolio)
        {
            try
            {
                var metrics = new PortfolioMetrics
                {
                    ExpectedReturn = portfolio.ExpectedReturn,
                    Risk = await _riskManager.CalculatePortfolioRiskAsync(portfolio),
                    MaxDrawdown = await _riskManager.CalculateMaxDrawdownAsync(portfolio),
                    AverageOdds = portfolio.Bets.Average(b => b.Odds),
                    WinRate = CalculateWinRate(portfolio),
                    KellyScore = portfolio.Bets.Average(b => b.StakePercent),
                    DiversificationIndex = CalculateDiversificationIndex(portfolio)
                };

                metrics.SharpeRatio = metrics.Risk > 0 ? metrics.ExpectedReturn / metrics.Risk : 0;

                return metrics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating portfolio metrics");
                return new PortfolioMetrics();
            }
        }

        private async Task AddArbitrageBetsAsync(BettingPortfolio portfolio, List<ArbitrageOpportunity> opportunities, decimal maxRisk)
        {
            foreach (var opportunity in opportunities.Take(2)) // Máximo 2 arbitrajes
            {
                var bet = new PortfolioBet(
                    opportunity.BetType,
                    opportunity.Odds1,
                    opportunity.RecommendedStake1,
                    opportunity.RecommendedStake1 / 10000, // Asumiendo bankroll de 10k
                    opportunity.ProfitMargin * opportunity.RecommendedStake1
                );

                if (await _riskManager.ValidatePositionAsync(bet, portfolio))
                {
                    portfolio.AddBet(bet);
                    _logger.LogInformation("Added arbitrage bet: {BetType} @ {Odds:F2}",
                        bet.BetType, bet.Odds);
                }
            }
        }

        private async Task AddValueBetsAsync(BettingPortfolio portfolio, List<ValueBet> valueBets, decimal maxRisk)
        {
            foreach (var valueBet in valueBets.Take(5)) // Máximo 5 value bets
            {
                var stakeAmount = valueBet.KellyFraction * 10000; // Asumiendo bankroll de 10k
                var bet = new PortfolioBet(
                    valueBet.BetType,
                    valueBet.MarketOdds,
                    stakeAmount,
                    valueBet.KellyFraction,
                    valueBet.ExpectedValue
                );

                if (await _riskManager.ValidatePositionAsync(bet, portfolio))
                {
                    portfolio.AddBet(bet);
                    _logger.LogInformation("Added value bet: {BetType} @ {Odds:F2} (EV: {EV:C})",
                        bet.BetType, bet.Odds, bet.ExpectedReturn);
                }
            }
        }

        private async Task ApplyDiversificationAsync(BettingPortfolio portfolio, Match match, MatchPredictions predictions, List<MarketOdds> availableOdds)
        {
            // Aplicar factores específicos del fútbol peruano
            if (match.IsHighAltitude())
            {
                // Favorecer Over 2.5 en partidos de altura
                var over25Bet = portfolio.Bets.FirstOrDefault(b => b.BetType == BetType.Over25);
                if (over25Bet != null)
                {
                    over25Bet.StakeAmount *= 1.2m; // 20% más stake
                    over25Bet.StakePercent *= 1.2m;
                }
            }

            if (match.TravelDistance > 1000)
            {
                // Favorecer al equipo local en viajes largos
                var homeWinBet = portfolio.Bets.FirstOrDefault(b => b.BetType == BetType.HomeWin);
                if (homeWinBet != null)
                {
                    homeWinBet.StakeAmount *= 1.15m; // 15% más stake
                    homeWinBet.StakePercent *= 1.15m;
                }
            }

            await Task.CompletedTask;
        }

        private async Task ValidateAndAdjustRiskAsync(BettingPortfolio portfolio, decimal maxRisk)
        {
            var currentRisk = await _riskManager.CalculatePortfolioRiskAsync(portfolio);
            
            if (currentRisk > maxRisk)
            {
                // Reducir stakes proporcionalmente
                var reductionFactor = maxRisk / currentRisk;
                foreach (var bet in portfolio.Bets)
                {
                    bet.StakeAmount *= reductionFactor;
                    bet.StakePercent *= reductionFactor;
                }

                _logger.LogWarning("Portfolio risk reduced from {CurrentRisk:P2} to {MaxRisk:P2}",
                    currentRisk, maxRisk);
            }
        }

        private decimal GetProbabilityForBetType(BetType betType, MatchPredictions predictions)
        {
            return betType switch
            {
                BetType.HomeWin => (decimal)predictions.HomeWinProbability,
                BetType.Draw => (decimal)predictions.DrawProbability,
                BetType.AwayWin => (decimal)predictions.AwayWinProbability,
                BetType.Over25 => (decimal)predictions.Over25Probability,
                BetType.Under25 => 1 - (decimal)predictions.Over25Probability,
                BetType.BothTeamsToScore => (decimal)predictions.BothScoreProbability,
                BetType.BothTeamsNotToScore => 1 - (decimal)predictions.BothScoreProbability,
                _ => 0.5m
            };
        }

        private async Task<decimal> CalculateDiversificationRiskAsync(List<ValueBet> valueBets, Match match)
        {
            if (valueBets.Count <= 1) return 0;

            var totalCorrelation = 0m;
            var pairCount = 0;

            for (int i = 0; i < valueBets.Count; i++)
            {
                for (int j = i + 1; j < valueBets.Count; j++)
                {
                    var correlation = await _arbitrageEngine.CalculateBetCorrelationAsync(
                        valueBets[i].BetType, valueBets[j].BetType, match);
                    totalCorrelation += Math.Abs(correlation);
                    pairCount++;
                }
            }

            return pairCount > 0 ? totalCorrelation / pairCount : 0;
        }

        private decimal CalculateWinRate(BettingPortfolio portfolio)
        {
            // Simular win rate basado en EV promedio
            var avgEV = portfolio.Bets.Average(b => b.ExpectedReturn);
            return Math.Max(0.3m, Math.Min(0.8m, 0.5m + avgEV / 1000)); // Entre 30% y 80%
        }

        private decimal CalculateDiversificationIndex(BettingPortfolio portfolio)
        {
            var betTypes = portfolio.Bets.Select(b => b.BetType).Distinct().Count();
            var totalBets = portfolio.Bets.Count;
            return totalBets > 0 ? (decimal)betTypes / totalBets : 0;
        }
    }
}
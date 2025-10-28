using ExciteStage.Application.Services;
using ExciteStage.Application.Services.HFT;
using ExciteStage.Domain.Entities;
using ExciteStage.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace ExciteStage.Infrastructure.Services.HFT
{
    /// <summary>
    /// Motor de arbitraje implementando detección de valor y oportunidades
    /// </summary>
    public class ArbitrageEngine : IArbitrageEngine
    {
        private readonly ILogger<ArbitrageEngine> _logger;
        private readonly List<string> _supportedBookies = new() 
        { 
            "Bet365", "William Hill", "Betfair", "Pinnacle", "SBObet" 
        };

        public ArbitrageEngine(ILogger<ArbitrageEngine> logger)
        {
            _logger = logger;
        }

        public async Task<List<ArbitrageOpportunity>> FindArbitrageOpportunitiesAsync(Match match)
        {
            try
            {
                var opportunities = new List<ArbitrageOpportunity>();

                // Simular obtención de odds de diferentes bookies
                var bookieOdds = await GetBookieOddsAsync(match);

                // Buscar arbitraje en 1X2
                var matchOdds = bookieOdds.Where(o => o.BetType == BetType.HomeWin || 
                                                    o.BetType == BetType.Draw || 
                                                    o.BetType == BetType.AwayWin).ToList();

                if (matchOdds.Count >= 3)
                {
                    var arbitrage = FindMatchArbitrage(matchOdds);
                    if (arbitrage != null)
                    {
                        opportunities.Add(arbitrage);
                    }
                }

                // Buscar arbitraje en Over/Under
                var overUnderOdds = bookieOdds.Where(o => o.BetType == BetType.Over25 || 
                                                        o.BetType == BetType.Under25).ToList();

                if (overUnderOdds.Count >= 2)
                {
                    var arbitrage = FindOverUnderArbitrage(overUnderOdds);
                    if (arbitrage != null)
                    {
                        opportunities.Add(arbitrage);
                    }
                }

                _logger.LogInformation("Found {Count} arbitrage opportunities for match {MatchId}",
                    opportunities.Count, match.Id);

                return opportunities;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finding arbitrage opportunities");
                return new List<ArbitrageOpportunity>();
            }
        }

        public async Task<decimal> CalculateExpectedValueAsync(
            decimal probability, 
            decimal odds, 
            decimal stake)
        {
            try
            {
                // EV = (probabilidad × ganancia) - (probabilidad × pérdida)
                var winProbability = probability;
                var lossProbability = 1 - probability;
                var winAmount = stake * (odds - 1);
                var lossAmount = stake;

                var expectedValue = (winProbability * winAmount) - (lossProbability * lossAmount);

                _logger.LogDebug("EV calculation: P={P:P2}, Odds={Odds:F2}, Stake={Stake:C}, EV={EV:C}",
                    probability, odds, stake, expectedValue);

                return await Task.FromResult(expectedValue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating expected value");
                return 0;
            }
        }

        public async Task<List<ValueBet>> FindValueBetsAsync(
            Match match, 
            MatchPredictions predictions, 
            List<MarketOdds> availableOdds)
        {
            try
            {
                var valueBets = new List<ValueBet>();

                foreach (var odds in availableOdds)
                {
                    var probability = GetProbabilityForBetType(odds.BetType, predictions);
                    var fairOdds = 1 / probability;
                    var expectedValue = await CalculateExpectedValueAsync(probability, odds.Odds, 100); // Stake de ejemplo

                    // Solo incluir apuestas con EV positivo
                    if (expectedValue > 0)
                    {
                        var kellyFraction = CalculateKellyFraction(probability, odds.Odds);

                        valueBets.Add(new ValueBet
                        {
                            BetType = odds.BetType,
                            Probability = probability,
                            MarketOdds = odds.Odds,
                            FairOdds = fairOdds,
                            ExpectedValue = expectedValue,
                            KellyFraction = kellyFraction,
                            Bookie = odds.Bookie
                        });
                    }
                }

                // Ordenar por EV descendente
                valueBets = valueBets.OrderByDescending(vb => vb.ExpectedValue).ToList();

                _logger.LogInformation("Found {Count} value bets for match {MatchId}",
                    valueBets.Count, match.Id);

                return valueBets;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finding value bets");
                return new List<ValueBet>();
            }
        }

        public async Task<decimal> CalculateBetCorrelationAsync(
            BetType betType1, 
            BetType betType2, 
            Match match)
        {
            try
            {
                // Calcular correlación basada en factores del partido
                var baseCorrelation = GetBaseCorrelation(betType1, betType2);
                
                // Ajustar por factores específicos del partido
                var altitudeFactor = match.IsHighAltitude() ? 0.1m : 0m;
                var travelFactor = match.TravelDistance > 1000 ? 0.05m : 0m;

                var adjustedCorrelation = baseCorrelation + altitudeFactor + travelFactor;

                return await Task.FromResult(Math.Max(-1, Math.Min(1, adjustedCorrelation)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating bet correlation");
                return 0;
            }
        }

        private async Task<List<MarketOdds>> GetBookieOddsAsync(Match match)
        {
            // Simular obtención de odds de diferentes bookies
            // En producción, esto se conectaría a APIs reales
            var odds = new List<MarketOdds>();

            foreach (var bookie in _supportedBookies)
            {
                // Simular odds con pequeñas variaciones
                var baseOdds = GetBaseOddsForMatch(match);
                var variation = (decimal)(new Random().NextDouble() - 0.5) * 0.1m; // ±5% variación

                odds.Add(new MarketOdds
                {
                    Bookie = bookie,
                    BetType = BetType.HomeWin,
                    Odds = baseOdds.HomeWin + variation,
                    LastUpdated = DateTime.UtcNow,
                    IsLive = false
                });

                odds.Add(new MarketOdds
                {
                    Bookie = bookie,
                    BetType = BetType.Draw,
                    Odds = baseOdds.Draw + variation,
                    LastUpdated = DateTime.UtcNow,
                    IsLive = false
                });

                odds.Add(new MarketOdds
                {
                    Bookie = bookie,
                    BetType = BetType.AwayWin,
                    Odds = baseOdds.AwayWin + variation,
                    LastUpdated = DateTime.UtcNow,
                    IsLive = false
                });
            }

            return await Task.FromResult(odds);
        }

        private (decimal HomeWin, decimal Draw, decimal AwayWin) GetBaseOddsForMatch(Match match)
        {
            // Simular odds basadas en factores del partido
            var altitudeBonus = match.IsHighAltitude() ? 0.1m : 0m;
            var travelPenalty = match.TravelDistance > 1000 ? 0.05m : 0m;

            return (
                HomeWin: 2.1m + altitudeBonus - travelPenalty,
                Draw: 3.2m,
                AwayWin: 3.5m + travelPenalty - altitudeBonus
            );
        }

        private ArbitrageOpportunity? FindMatchArbitrage(List<MarketOdds> odds)
        {
            // Buscar la mejor combinación de odds para 1X2
            var homeWinOdds = odds.Where(o => o.BetType == BetType.HomeWin).OrderByDescending(o => o.Odds).FirstOrDefault();
            var drawOdds = odds.Where(o => o.BetType == BetType.Draw).OrderByDescending(o => o.Odds).FirstOrDefault();
            var awayWinOdds = odds.Where(o => o.BetType == BetType.AwayWin).OrderByDescending(o => o.Odds).FirstOrDefault();

            if (homeWinOdds == null || drawOdds == null || awayWinOdds == null)
                return null;

            // Calcular si hay arbitraje: 1/odds1 + 1/odds2 + 1/odds3 < 1
            var totalImpliedProbability = (1 / homeWinOdds.Odds) + (1 / drawOdds.Odds) + (1 / awayWinOdds.Odds);

            if (totalImpliedProbability < 1)
            {
                var profitMargin = 1 - totalImpliedProbability;
                
                return new ArbitrageOpportunity
                {
                    Bookie1 = homeWinOdds.Bookie,
                    Bookie2 = drawOdds.Bookie,
                    BetType = BetType.HomeWin, // Arbitraje en 1X2
                    Odds1 = homeWinOdds.Odds,
                    Odds2 = drawOdds.Odds,
                    ProfitMargin = profitMargin,
                    RecommendedStake1 = 100 * (1 / homeWinOdds.Odds) / totalImpliedProbability,
                    RecommendedStake2 = 100 * (1 / drawOdds.Odds) / totalImpliedProbability,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(5) // Arbitraje expira rápido
                };
            }

            return null;
        }

        private ArbitrageOpportunity? FindOverUnderArbitrage(List<MarketOdds> odds)
        {
            var overOdds = odds.Where(o => o.BetType == BetType.Over25).OrderByDescending(o => o.Odds).FirstOrDefault();
            var underOdds = odds.Where(o => o.BetType == BetType.Under25).OrderByDescending(o => o.Odds).FirstOrDefault();

            if (overOdds == null || underOdds == null)
                return null;

            var totalImpliedProbability = (1 / overOdds.Odds) + (1 / underOdds.Odds);

            if (totalImpliedProbability < 1)
            {
                var profitMargin = 1 - totalImpliedProbability;
                
                return new ArbitrageOpportunity
                {
                    Bookie1 = overOdds.Bookie,
                    Bookie2 = underOdds.Bookie,
                    BetType = BetType.Over25,
                    Odds1 = overOdds.Odds,
                    Odds2 = underOdds.Odds,
                    ProfitMargin = profitMargin,
                    RecommendedStake1 = 100 * (1 / overOdds.Odds) / totalImpliedProbability,
                    RecommendedStake2 = 100 * (1 / underOdds.Odds) / totalImpliedProbability,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(5)
                };
            }

            return null;
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

        private decimal CalculateKellyFraction(decimal probability, decimal odds)
        {
            var b = odds - 1;
            var p = probability;
            var q = 1 - p;
            return Math.Max(0, (b * p - q) / b);
        }

        private decimal GetBaseCorrelation(BetType betType1, BetType betType2)
        {
            return (betType1, betType2) switch
            {
                (BetType.HomeWin, BetType.AwayWin) => -0.8m,
                (BetType.Over25, BetType.Under25) => -1.0m,
                (BetType.BothTeamsToScore, BetType.BothTeamsNotToScore) => -1.0m,
                (BetType.HomeWin, BetType.Over25) => 0.3m,
                (BetType.AwayWin, BetType.Over25) => 0.3m,
                (BetType.BothTeamsToScore, BetType.Over25) => 0.6m,
                _ => 0.1m
            };
        }
    }
}


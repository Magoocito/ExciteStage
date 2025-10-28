using ExciteStage.Domain.Entities;
using ExciteStage.Domain.ValueObjects;

namespace ExciteStage.Application.Services.HFT
{
    /// <summary>
    /// Motor de arbitraje para detectar oportunidades de valor en apuestas.
    /// Basado en principios de HFT y market making.
    /// </summary>
    public interface IArbitrageEngine
    {
        /// <summary>
        /// Detecta oportunidades de arbitraje entre diferentes bookies
        /// </summary>
        Task<List<ArbitrageOpportunity>> FindArbitrageOpportunitiesAsync(Match match);

        /// <summary>
        /// Calcula el Expected Value (EV) de una apuesta
        /// </summary>
        Task<decimal> CalculateExpectedValueAsync(
            decimal probability, 
            decimal odds, 
            decimal stake);

        /// <summary>
        /// Encuentra apuestas con valor positivo (EV > 0)
        /// </summary>
        Task<List<ValueBet>> FindValueBetsAsync(
            Match match, 
            MatchPredictions predictions, 
            List<MarketOdds> availableOdds);

        /// <summary>
        /// Calcula la correlación entre diferentes tipos de apuestas
        /// </summary>
        Task<decimal> CalculateBetCorrelationAsync(
            BetType betType1, 
            BetType betType2, 
            Match match);
    }

    /// <summary>
    /// Oportunidad de arbitraje detectada
    /// </summary>
    public class ArbitrageOpportunity
    {
        public string Bookie1 { get; set; } = string.Empty;
        public string Bookie2 { get; set; } = string.Empty;
        public BetType BetType { get; set; }
        public decimal Odds1 { get; set; }
        public decimal Odds2 { get; set; }
        public decimal ProfitMargin { get; set; }
        public decimal RecommendedStake1 { get; set; }
        public decimal RecommendedStake2 { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    /// <summary>
    /// Apuesta con valor positivo identificada
    /// </summary>
    public class ValueBet
    {
        public BetType BetType { get; set; }
        public decimal Probability { get; set; }
        public decimal MarketOdds { get; set; }
        public decimal FairOdds { get; set; }
        public decimal ExpectedValue { get; set; }
        public decimal KellyFraction { get; set; }
        public string Bookie { get; set; } = string.Empty;
    }

    /// <summary>
    /// Odds de mercado de un bookie
    /// </summary>
    public class MarketOdds
    {
        public string Bookie { get; set; } = string.Empty;
        public BetType BetType { get; set; }
        public decimal Odds { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool IsLive { get; set; }
    }

}

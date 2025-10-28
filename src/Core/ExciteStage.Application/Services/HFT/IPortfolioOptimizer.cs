using ExciteStage.Domain.Entities;
using ExciteStage.Domain.ValueObjects;

namespace ExciteStage.Application.Services.HFT
{
    /// <summary>
    /// Optimizador de portafolio estilo HFT para apuestas de fútbol.
    /// Combina ML predictions con gestión de riesgo avanzada.
    /// </summary>
    public interface IPortfolioOptimizer
    {
        /// <summary>
        /// Optimiza un portafolio completo usando algoritmos HFT
        /// </summary>
        Task<BettingPortfolio> OptimizePortfolioAsync(
            Match match,
            MatchPredictions predictions,
            List<MarketOdds> availableOdds,
            decimal maxRiskPercent = 0.15);

        /// <summary>
        /// Rebalancea un portafolio existente basado en cambios de odds
        /// </summary>
        Task<BettingPortfolio> RebalancePortfolioAsync(
            BettingPortfolio currentPortfolio,
            List<MarketOdds> updatedOdds,
            MatchPredictions updatedPredictions);

        /// <summary>
        /// Calcula la diversificación óptima del portafolio
        /// </summary>
        Task<PortfolioDiversification> CalculateOptimalDiversificationAsync(
            Match match,
            MatchPredictions predictions,
            List<MarketOdds> availableOdds);

        /// <summary>
        /// Ejecuta stop-loss automático si es necesario
        /// </summary>
        Task<PortfolioAction> ExecuteStopLossAsync(BettingPortfolio portfolio);

        /// <summary>
        /// Calcula métricas de rendimiento del portafolio
        /// </summary>
        Task<PortfolioMetrics> CalculatePortfolioMetricsAsync(BettingPortfolio portfolio);
    }

    /// <summary>
    /// Estrategia de diversificación del portafolio
    /// </summary>
    public class PortfolioDiversification
    {
        public decimal HomeWinAllocation { get; set; }
        public decimal DrawAllocation { get; set; }
        public decimal AwayWinAllocation { get; set; }
        public decimal OverUnderAllocation { get; set; }
        public decimal BTTSAllocation { get; set; }
        public decimal TotalAllocation { get; set; }
        public decimal RiskScore { get; set; }
        public List<string> RecommendedBets { get; set; } = new();
    }

    /// <summary>
    /// Acción a tomar en el portafolio
    /// </summary>
    public class PortfolioAction
    {
        public ActionType Type { get; set; }
        public string Reason { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public List<PortfolioBet> BetsToClose { get; set; } = new();
        public List<PortfolioBet> BetsToAdd { get; set; } = new();
    }

    /// <summary>
    /// Tipos de acciones del portafolio
    /// </summary>
    public enum ActionType
    {
        Hold,
        Rebalance,
        StopLoss,
        TakeProfit,
        Hedge
    }

    /// <summary>
    /// Métricas de rendimiento del portafolio
    /// </summary>
    public class PortfolioMetrics
    {
        public decimal ExpectedReturn { get; set; }
        public decimal Risk { get; set; }
        public decimal SharpeRatio { get; set; }
        public decimal MaxDrawdown { get; set; }
        public decimal WinRate { get; set; }
        public decimal AverageOdds { get; set; }
        public decimal KellyScore { get; set; }
        public decimal DiversificationIndex { get; set; }
    }
}

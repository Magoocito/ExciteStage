using ExciteStage.Domain.Entities;

namespace ExciteStage.Application.Services.HFT
{
    /// <summary>
    /// Gestor de riesgo estilo HFT para portafolios de apuestas.
    /// Implementa Kelly Criterion y gestión de exposición.
    /// </summary>
    public interface IRiskManager
    {
        /// <summary>
        /// Calcula el tamaño óptimo de posición usando Kelly Criterion
        /// </summary>
        Task<decimal> CalculateOptimalPositionSizeAsync(
            decimal probability, 
            decimal odds, 
            decimal bankroll, 
            decimal currentExposure);

        /// <summary>
        /// Valida si una apuesta cumple con los límites de riesgo
        /// </summary>
        Task<bool> ValidatePositionAsync(PortfolioBet bet, BettingPortfolio portfolio);

        /// <summary>
        /// Calcula el riesgo total del portafolio
        /// </summary>
        Task<decimal> CalculatePortfolioRiskAsync(BettingPortfolio portfolio);

        /// <summary>
        /// Determina si se debe activar stop-loss
        /// </summary>
        Task<bool> ShouldTriggerStopLossAsync(BettingPortfolio portfolio, decimal currentValue);

        /// <summary>
        /// Calcula el máximo drawdown permitido
        /// </summary>
        Task<decimal> CalculateMaxDrawdownAsync(BettingPortfolio portfolio);
    }
}

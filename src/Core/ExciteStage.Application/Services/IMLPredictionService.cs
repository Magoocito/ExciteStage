using ExciteStage.Domain.Entities;

namespace ExciteStage.Application.Services
{
    public interface IMLPredictionService
    {
        Task<MatchPredictions> GeneratePredictionsAsync(Match match);
        Task<ExpertSentiment> GetExpertOpinionAsync(Match match);
    }
}

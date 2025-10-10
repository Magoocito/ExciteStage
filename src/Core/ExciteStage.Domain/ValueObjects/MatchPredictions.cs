namespace ExciteStage.Application.Services
{
    public record MatchPredictions
    {
        public double HomeWinProbability { get; init; }
        public double DrawProbability { get; init; }
        public double AwayWinProbability { get; init; }
        public double Over25Probability { get; init; }
        public double BothScoreProbability { get; init; }
        public double Confidence { get; init; }
    }
}

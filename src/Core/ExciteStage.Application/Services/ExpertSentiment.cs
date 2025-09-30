namespace ExciteStage.Application.Services
{
    public record ExpertSentiment
    {
        public double OverallSentiment { get; init; }
        public int ExpertCount { get; init; }
        public double Confidence { get; init; }
    }
}

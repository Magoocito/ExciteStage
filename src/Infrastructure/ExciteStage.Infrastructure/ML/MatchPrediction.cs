namespace ExciteStage.Infrastructure.ML
{
    public class MatchPrediction
    {
        public float HomeWinProbability { get; set; }
        public float DrawProbability { get; set; }
        public float AwayWinProbability { get; set; }
        public float Over25Probability { get; set; }
        public float BothScoreProbability { get; set; }
        public float Confidence { get; set; }
    }
}

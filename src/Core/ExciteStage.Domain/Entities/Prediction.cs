namespace ExciteStage.Domain.Entities
{
    public sealed class Prediction
    {
        public int MatchId { get; set; }
        public double WinProbability { get; set; }
        public bool IsPositiveEV => WinProbability > 0.05;

        public Prediction(int matchId, double winProbability)
        {
            if (winProbability < 0 || winProbability > 1)
                throw new ArgumentException("WinProbability must be between 0 and 1.", nameof(winProbability));
            MatchId = matchId;
            WinProbability = winProbability;
        }

        public void SetWinProbability(double value)
        {
            if (value < 0 || value > 1)
                throw new ArgumentException("WinProbability must be between 0 and 1.", nameof(value));
            WinProbability = value;
        }
    }
}

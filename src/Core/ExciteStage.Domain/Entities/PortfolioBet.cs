using System;

namespace ExciteStage.Domain.Entities
{
    public sealed class PortfolioBet
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Market { get; set; }
        public double Odds { get; set; }
        public double StakePercent { get; set; }
        public double Confidence { get; set; }
        public double ExpectedReturn { get; set; }
        public string Reasoning { get; set; }

        public PortfolioBet(string type, string market, double odds, double stakePercent, double confidence, string reasoning)
        {
            Type = type;
            Market = market;
            Odds = odds;
            StakePercent = stakePercent;
            Confidence = confidence;
            Reasoning = reasoning;
            ExpectedReturn = (odds - 1) * stakePercent;
        }

        private PortfolioBet() { }
    }
}

using System;
using ExciteStage.Domain.ValueObjects;

namespace ExciteStage.Domain.Entities
{
    public sealed class PortfolioBet
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Market { get; set; }
        public decimal Odds { get; set; }
        public decimal StakePercent { get; set; }
        public decimal Confidence { get; set; }
        public decimal ExpectedReturn { get; set; }
        public string Reasoning { get; set; }
        public BetType BetType { get; set; }
        public decimal StakeAmount { get; set; }

        public PortfolioBet(string type, string market, decimal odds, decimal stakePercent, decimal confidence, string reasoning)
        {
            Type = type;
            Market = market;
            Odds = odds;
            StakePercent = stakePercent;
            Confidence = confidence;
            Reasoning = reasoning;
            ExpectedReturn = (odds - 1) * stakePercent;
        }

        public PortfolioBet(BetType betType, decimal odds, decimal stakeAmount, decimal stakePercent, decimal expectedReturn)
        {
            BetType = betType;
            Odds = odds;
            StakeAmount = stakeAmount;
            StakePercent = stakePercent;
            ExpectedReturn = expectedReturn;
            Type = betType.ToString();
            Market = "HFT";
            Confidence = 0.8m;
            Reasoning = "HFT Optimized";
        }

        private PortfolioBet() { }
    }
}

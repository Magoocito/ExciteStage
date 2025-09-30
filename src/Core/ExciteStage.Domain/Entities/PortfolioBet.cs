using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExciteStage.Domain.Entities
{
    public sealed class PortfolioBet
    {
        public string BetType { get; set; }
        public string Market { get; set; }
        public double Odds { get; set; }
        public double StakePercent { get; set; }
        public double Confidence { get; set; }
        public double ExpectedReturn { get; set; }
        public string Reasoning { get; set; }

        public PortfolioBet(string betType, string market, double odds, double stakePercent, double confidence, string reasoning)
        {
            BetType = betType;
            Market = market;
            Odds = odds;
            StakePercent = stakePercent;
            Confidence = confidence;
            Reasoning = reasoning;
            ExpectedReturn = (odds - 1) * stakePercent;
        }
    }
}

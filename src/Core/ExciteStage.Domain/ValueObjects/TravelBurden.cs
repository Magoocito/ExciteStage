using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExciteStage.Domain.ValueObjects
{
    public sealed class TravelBurden
    {
        public int DistanceKm { get; }
        public double FatigueImpact { get; }

        public TravelBurden(int distanceKm)
        {
            DistanceKm = distanceKm;
            FatigueImpact = distanceKm switch
            {
                < 300 => 0.0,
                >= 300 and < 700 => 0.05,
                >= 700 and < 1200 => 0.10,
                >= 1200 => 0.15
            };
        }

        public bool IsHighImpact() => FatigueImpact > 0.08;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExciteStage.Domain.ValueObjects
{
    public sealed class AltitudeFactor
    {
        public int HomeAltitude { get; }
        public int AwayAltitude { get; }
        public double ImpactFactor { get; }

        public AltitudeFactor(int homeAltitude, int awayAltitude)
        {
            HomeAltitude = homeAltitude;
            AwayAltitude = awayAltitude;
            var difference = Math.Abs(homeAltitude - awayAltitude);
            ImpactFactor = difference > 1000 ? Math.Min(0.25, difference / 4000.0) : 0.0;
        }

        public bool IsSignificant() => ImpactFactor > 0.05;
    }
}

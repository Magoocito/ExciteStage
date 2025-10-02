using System;
using System.Collections.Generic;

namespace ExciteStage.Domain.Entities
{
    public sealed class Match
    {
        public int Id { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public DateTime MatchDate { get; set; } // NUEVO
        public int Altitude { get; set; }
        public int TravelDistance { get; set; }
        public int RefereeBias { get; set; }
        public string WeatherImpact { get; set; }

        // NUEVO: Relación con portfolios
        public ICollection<BettingPortfolio> Portfolios { get; set; } = new List<BettingPortfolio>();

        public Match(int id, string homeTeam, string awayTeam, DateTime matchDate, int altitude = 0, int travelDistance = 0, int refereeBias = 0, string weatherImpact = "Unknown")
        {
            if (string.IsNullOrWhiteSpace(homeTeam))
                throw new ArgumentException("HomeTeam cannot be empty.", nameof(homeTeam));
            if (string.IsNullOrWhiteSpace(awayTeam))
                throw new ArgumentException("AwayTeam cannot be empty.", nameof(awayTeam));
            if (altitude < 0)
                throw new ArgumentException("Altitude cannot be negative.", nameof(altitude));
            if (travelDistance < 0)
                throw new ArgumentException("TravelDistance cannot be negative.", nameof(travelDistance));
            if (refereeBias < 0 || refereeBias > 10)
                throw new ArgumentException("RefereeBias must be between 0 and 10.", nameof(refereeBias));
            if (string.IsNullOrWhiteSpace(weatherImpact))
                throw new ArgumentException("WeatherImpact cannot be empty.", nameof(weatherImpact));

            Id = id;
            HomeTeam = homeTeam;
            AwayTeam = awayTeam;
            MatchDate = matchDate;
            Altitude = altitude;
            TravelDistance = travelDistance;
            RefereeBias = refereeBias;
            WeatherImpact = weatherImpact;
        }

        public bool IsHighAltitude() => Altitude > 2500;

        public override string ToString()
        {
            return $"{HomeTeam} vs {AwayTeam} ({MatchDate:yyyy-MM-dd}), Alt: {Altitude}m, Dist: {TravelDistance}km, RefBias: {RefereeBias}, Weather: {WeatherImpact}";
        }
    }
}

namespace FutbolExitante.Domain.Entities
{
    public class Match
    {
        public int Id { get; set; }
        public string HomeTeam { get; set; } = string.Empty;
        public string AwayTeam { get; set; } = string.Empty;
        public int Altitude { get; set; } // en metros
        public int TravelDistance { get; set; } // en km

        public bool IsHighAltitude()
        {
            return Altitude > 2500;
        }
    }
}
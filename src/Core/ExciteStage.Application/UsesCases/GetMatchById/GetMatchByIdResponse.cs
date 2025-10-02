namespace ExciteStage.Application.UseCases.GetMatchById
{
    public class GetMatchByIdResponse
    {
        public int Id { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public DateTime MatchDate { get; set; }
        // Agrega más propiedades si lo necesitas
    }
}
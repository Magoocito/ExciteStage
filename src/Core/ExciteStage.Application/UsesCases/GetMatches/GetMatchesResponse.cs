using System.Collections.Generic;

namespace ExciteStage.Application.UseCases.GetMatches
{
    public class GetMatchesResponse
    {
        public List<MatchDto> Matches { get; set; } = new();
    }

    public class MatchDto
    {
        public int Id { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public DateTime MatchDate { get; set; }
    }
}
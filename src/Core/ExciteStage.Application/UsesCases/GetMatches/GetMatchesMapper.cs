using AutoMapper;
using ExciteStage.Domain.Entities;

namespace ExciteStage.Application.UseCases.GetMatches
{
    public class GetMatchesMapper : Profile
    {
        public GetMatchesMapper()
        {
            CreateMap<Match, MatchDto>();
        }
    }
}
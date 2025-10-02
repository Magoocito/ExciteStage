using AutoMapper;
using ExciteStage.Domain.Entities;

namespace ExciteStage.Application.UseCases.GetMatchById
{
    public class GetMatchByIdMapper : Profile
    {
        public GetMatchByIdMapper()
        {
            CreateMap<Match, GetMatchByIdResponse>();
        }
    }
}
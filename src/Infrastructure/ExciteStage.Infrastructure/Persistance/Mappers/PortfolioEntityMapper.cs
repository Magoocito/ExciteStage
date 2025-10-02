using AutoMapper;
using ExciteStage.Infrastructure.Persistance.Entities;
using ExciteStage.Domain.Entities;

namespace ExciteStage.Infrastructure.Persistance.Mappers
{
    public class PortfolioEntityMapper : Profile
    {
        public PortfolioEntityMapper()
        {
            CreateMap<BettingPortfolioEntity, BettingPortfolio>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.MatchId, opt => opt.MapFrom(src => src.MatchId))
                .ForMember(dest => dest.TotalStakePercent, opt => opt.MapFrom(src => src.TotalStakePercent))
                .ForMember(dest => dest.ExpectedReturn, opt => opt.MapFrom(src => src.ExpectedReturn))
                .ForMember(dest => dest.Bets, opt => opt.MapFrom(src => src.Bets));

            CreateMap<BettingPortfolio, BettingPortfolioEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.MatchId, opt => opt.MapFrom(src => src.MatchId))
                .ForMember(dest => dest.TotalStakePercent, opt => opt.MapFrom(src => src.TotalStakePercent))
                .ForMember(dest => dest.ExpectedReturn, opt => opt.MapFrom(src => src.ExpectedReturn));
        }
    }
}
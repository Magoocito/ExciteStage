using AutoMapper;
using ExciteStage.Domain.Entities;

namespace ExciteStage.Application.UseCases.CreatePortfolio
{
    public class CreatePortfolioMapper : Profile
    {
        public CreatePortfolioMapper()
        {
            CreateMap<BettingPortfolio, CreatePortfolioResponse>()
                .ForMember(dest => dest.PortfolioId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.TotalRiskPercent, opt => opt.MapFrom(src => src.TotalStakePercent))
                .ForMember(dest => dest.ExpectedReturn, opt => opt.MapFrom(src => src.ExpectedReturn))
                .ForMember(dest => dest.Bets, opt => opt.MapFrom(src => src.Bets));

            CreateMap<PortfolioBet, BetDto>();
        }
    }
}
using AutoMapper;
using ExciteStage.Infrastructure.Persistance.Entities;
using ExciteStage.Domain.Entities;

namespace ExciteStage.Infrastructure.Persistance.Mappers
{
    public class PortfolioBetEntityMapper : Profile
    {
        public PortfolioBetEntityMapper()
        {
            CreateMap<PortfolioBetEntity, PortfolioBet>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.Market, opt => opt.MapFrom(src => src.Market))
                .ForMember(dest => dest.Odds, opt => opt.MapFrom(src => src.Odds))
                .ForMember(dest => dest.StakePercent, opt => opt.MapFrom(src => src.StakePercent))
                .ForMember(dest => dest.Confidence, opt => opt.MapFrom(src => src.Confidence))
                .ForMember(dest => dest.ExpectedReturn, opt => opt.MapFrom(src => src.ExpectedReturn))
                .ForMember(dest => dest.Reasoning, opt => opt.MapFrom(src => src.Reasoning));

            CreateMap<PortfolioBet, PortfolioBetEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.Market, opt => opt.MapFrom(src => src.Market))
                .ForMember(dest => dest.Odds, opt => opt.MapFrom(src => src.Odds))
                .ForMember(dest => dest.StakePercent, opt => opt.MapFrom(src => src.StakePercent))
                .ForMember(dest => dest.Confidence, opt => opt.MapFrom(src => src.Confidence))
                .ForMember(dest => dest.ExpectedReturn, opt => opt.MapFrom(src => src.ExpectedReturn))
                .ForMember(dest => dest.Reasoning, opt => opt.MapFrom(src => src.Reasoning));
        }
    }
}
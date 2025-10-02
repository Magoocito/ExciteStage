using AutoMapper;
using ExciteStage.Domain.Entities;
using ExciteStage.Infrastructure.Persistance.Entities;

namespace ExciteStage.Infrastructure.Persistance.Mappers
{
    public class MatchEntityMapper : Profile
    {
        public MatchEntityMapper()
        {
            // Mapeo de entidad a dominio y viceversa
            CreateMap<MatchEntity, Match>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.HomeTeam, opt => opt.MapFrom(src => src.HomeTeam))
                .ForMember(dest => dest.AwayTeam, opt => opt.MapFrom(src => src.AwayTeam))
                .ForMember(dest => dest.MatchDate, opt => opt.MapFrom(src => src.MatchDate))
                .ForMember(dest => dest.Altitude, opt => opt.MapFrom(src => src.Altitude))
                .ForMember(dest => dest.TravelDistance, opt => opt.MapFrom(src => src.TravelDistance))
                // Si tienes portfolios en la entidad de dominio, puedes mapearlos también
                .ForMember(dest => dest.Portfolios, opt => opt.MapFrom(src => src.Portfolios));

            CreateMap<Match, MatchEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.HomeTeam, opt => opt.MapFrom(src => src.HomeTeam))
                .ForMember(dest => dest.AwayTeam, opt => opt.MapFrom(src => src.AwayTeam))
                .ForMember(dest => dest.MatchDate, opt => opt.MapFrom(src => src.MatchDate))
                .ForMember(dest => dest.Altitude, opt => opt.MapFrom(src => src.Altitude))
                .ForMember(dest => dest.TravelDistance, opt => opt.MapFrom(src => src.TravelDistance));
        }
    }
}
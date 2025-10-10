using AutoMapper;
using ExciteStage.Application.Services;
using ExciteStage.Domain.Entities;
using ExciteStage.Infrastructure.ML;
using ExciteStage.Infrastructure.ML.Features;

namespace ExciteStage.Infrastructure.Services.ML
{
    /// <summary>
    /// Profile de AutoMapper para transformaciones relacionadas con ML.
    /// Mapea entre Domain entities, ML features y predicciones.
    /// </summary>
    public class MLMappingProfile : Profile
    {
        public MLMappingProfile()
        {
            // ========================================
            // Match (Domain) → MatchFeatures (ML Input)
            // ========================================
            CreateMap<Match, MatchFeatures>()
                .ForMember(dest => dest.HomeAltitude, opt => opt.MapFrom(src => (float)src.Altitude))
                .ForMember(dest => dest.TravelDistance, opt => opt.MapFrom(src => (float)src.TravelDistance))
                .ForMember(dest => dest.IsHighAltitude, opt => opt.MapFrom(src => src.IsHighAltitude() ? 1f : 0f))
                .ForMember(dest => dest.RefereeBias, opt => opt.MapFrom(src => (float)src.RefereeBias))
                .ForMember(dest => dest.WeatherImpactEncoded, opt => opt.MapFrom(src => EncodeWeather(src.WeatherImpact)))
                // Campos adicionales que podrías agregar:
                .ForMember(dest => dest.HomeFormLast5, opt => opt.Ignore()) // TODO: Calcular desde históricos
                .ForMember(dest => dest.AwayFormLast5, opt => opt.Ignore())
                .ForMember(dest => dest.HeadToHeadWins, opt => opt.Ignore());

            // ========================================
            // MatchPrediction (ML Output) → MatchPredictions (Value Object/Record)
            // ========================================
            CreateMap<MatchPrediction, MatchPredictions>()
                .ForMember(dest => dest.HomeWinProbability, opt => opt.MapFrom(src => src.HomeWinProbability))
                .ForMember(dest => dest.DrawProbability, opt => opt.MapFrom(src => src.DrawProbability))
                .ForMember(dest => dest.AwayWinProbability, opt => opt.MapFrom(src => src.AwayWinProbability))
                .ForMember(dest => dest.Over25Probability, opt => opt.MapFrom(src => src.Over25Probability))
                .ForMember(dest => dest.BothScoreProbability, opt => opt.MapFrom(src => src.BothScoreProbability))
                .ForMember(dest => dest.Confidence, opt => opt.MapFrom(src => src.Confidence));
        }

        /// <summary>
        /// Codifica el impacto climático a un valor numérico para el modelo.
        /// </summary>
        private static float EncodeWeather(string weatherImpact)
        {
            return weatherImpact?.ToLower() switch
            {
                "clear" => 1.0f,
                "rain" => 0.7f,
                "heavy rain" => 0.5f,
                "extreme heat" => 0.6f,
                "fog" => 0.4f,
                _ => 0.8f // Default: condiciones normales
            };
        }
    }
}
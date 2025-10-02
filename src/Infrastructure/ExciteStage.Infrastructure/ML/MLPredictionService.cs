using ExciteStage.Domain.Entities;
using System.Text.Json;

namespace ExciteStage.Application.Services
{
    public class MLPredictionService : IMLPredictionService
    {
        private readonly HttpClient _httpClient;

        public MLPredictionService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<MatchPredictions> GeneratePredictionsAsync(Match match)
        {
            // Construye la URL del endpoint de tu API
            var apiUrl = $"https://api.tu-ml.net/predictions?matchId={match.Id}";

            // Realiza la llamada GET
            var response = await _httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            // Lee y deserializa el JSON
            var json = await response.Content.ReadAsStringAsync();
            var predictions = JsonSerializer.Deserialize<MatchPredictions>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (predictions == null)
                throw new InvalidOperationException("No se pudo obtener predicciones del API.");

            return predictions;
        }

        public async Task<ExpertSentiment> GetExpertOpinionAsync(Match match)
        {
            // Aquí puedes implementar la llamada a otro API para obtener el sentimiento experto
            return new ExpertSentiment
            {
                OverallSentiment = 0.0,
                ExpertCount = 0,
                Confidence = 0.5
            };
        }
    }
}
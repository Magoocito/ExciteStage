using RestSharp;
using System.Threading.Tasks;

namespace ExciteStage.Infrastructure.External
{
    public class ApiFootballClient
    {
        private readonly RestClient _client;
        private readonly string _apiKey;

        public ApiFootballClient(string apiKey)
        {
            _client = new RestClient("https://api-football-v3.p.rapidapi.com");
            _apiKey = apiKey;
        }

        public async Task<string?> GetFixtures(string leagueId, string season)
        {
            var request = new RestRequest("/fixtures", Method.Get);
            request.AddHeader("x-rapidapi-key", _apiKey);
            request.AddHeader("x-rapidapi-host", "api-football-v3.p.rapidapi.com"); // <-- Este header es importante
            request.AddParameter("league", leagueId);
            request.AddParameter("season", season);
            var response = await _client.ExecuteAsync(request);
            return response.Content;
        }
    }
}
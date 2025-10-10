using Newtonsoft.Json.Linq;
using RestSharp;

namespace ExciteStage.Infrastructure.Services.External
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
            request.AddHeader("x-rapidapi-host", "api-football-v3.p.rapidapi.com");
            request.AddParameter("league", leagueId);
            request.AddParameter("season", season);
            var response = await _client.ExecuteAsync(request);
            return response.Content;
        }

        // NUEVO: Exporta datos históricos a CSV
        public async Task ExportHistoricalFixturesToCsvAsync(string leagueId, string[] seasons, string outputPath)
        {
            var allFixtures = new List<FixtureData>();

            foreach (var season in seasons)
            {
                var json = await GetFixtures(leagueId, season);
                if (string.IsNullOrEmpty(json)) continue;

                var data = JObject.Parse(json);
                var fixtures = (JArray)data["response"];

                foreach (var fixture in fixtures)
                {
                    try
                    {
                        var fixtureData = new FixtureData
                        {
                            Date = fixture["fixture"]["date"].ToString(),
                            HomeTeam = fixture["teams"]["home"]["name"].ToString(),
                            AwayTeam = fixture["teams"]["away"]["name"].ToString(),
                            HomeGoals = int.Parse(fixture["goals"]["home"]?.ToString() ?? "0"),
                            AwayGoals = int.Parse(fixture["goals"]["away"]?.ToString() ?? "0"),
                            HomeAltitude = GetStadiumAltitude(fixture["teams"]["home"]["name"].ToString()),
                            TravelDistance = CalculateTravelDistance(
                                fixture["teams"]["home"]["name"].ToString(),
                                fixture["teams"]["away"]["name"].ToString()
                            )
                        };

                        allFixtures.Add(fixtureData);
                    }
                    catch { /* Ignorar errores de parsing individuales */ }
                }
            }

            using (var writer = new StreamWriter(outputPath))
            {
                writer.WriteLine("Date,HomeTeam,AwayTeam,HomeGoals,AwayGoals,HomeAltitude,TravelDistance,IsHighAltitude,HomeWin");
                foreach (var fixture in allFixtures)
                {
                    var isHighAltitude = fixture.HomeAltitude > 2500 ? 1 : 0;
                    var homeWin = fixture.HomeGoals > fixture.AwayGoals ? 1.0 :
                                 fixture.HomeGoals == fixture.AwayGoals ? 0.5 : 0.0;

                    writer.WriteLine(
                        $"{fixture.Date},{fixture.HomeTeam},{fixture.AwayTeam}," +
                        $"{fixture.HomeGoals},{fixture.AwayGoals},{fixture.HomeAltitude}," +
                        $"{fixture.TravelDistance},{isHighAltitude},{homeWin:F1}"
                    );
                }
            }
        }

        // Helpers para enriquecer datos
        private int GetStadiumAltitude(string teamName)
        {
            return teamName.ToLower() switch
            {
                var t when t.Contains("cusco") => 3400,
                var t when t.Contains("cienciano") => 3400,
                var t when t.Contains("melgar") => 2335,
                var t when t.Contains("binacional") => 3825,
                var t when t.Contains("garcilaso") => 3400,
                var t when t.Contains("ayacucho") => 2746,
                var t when t.Contains("huancayo") => 3259,
                var t when t.Contains("cajamarca") => 2750,
                var t when t.Contains("universitario") => 0,
                var t when t.Contains("alianza") => 0,
                var t when t.Contains("cristal") => 0,
                _ => 150 // Default coastal cities
            };
        }

        private int CalculateTravelDistance(string homeTeam, string awayTeam)
        {
            var cityDistances = new Dictionary<(string, string), int>
            {
                { ("Lima", "Cusco"), 1100 },
                { ("Lima", "Arequipa"), 1000 },
                { ("Lima", "Cajamarca"), 850 },
                { ("Lima", "Huancayo"), 300 },
                { ("Cusco", "Arequipa"), 600 },
                { ("Cusco", "Lima"), 1100 },
            };

            var homeCity = GetCityFromTeam(homeTeam);
            var awayCity = GetCityFromTeam(awayTeam);

            if (cityDistances.TryGetValue((awayCity, homeCity), out var distance))
                return distance;

            return homeCity == awayCity ? 0 : 500;
        }

        private string GetCityFromTeam(string teamName)
        {
            return teamName.ToLower() switch
            {
                var t when t.Contains("cusco") || t.Contains("cienciano") || t.Contains("garcilaso") => "Cusco",
                var t when t.Contains("melgar") => "Arequipa",
                var t when t.Contains("universitario") || t.Contains("alianza") || t.Contains("cristal") => "Lima",
                var t when t.Contains("cajamarca") => "Cajamarca",
                var t when t.Contains("huancayo") => "Huancayo",
                _ => "Lima"
            };
        }
    }

    public class FixtureData
    {
        public string Date { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public int HomeGoals { get; set; }
        public int AwayGoals { get; set; }
        public int HomeAltitude { get; set; }
        public int TravelDistance { get; set; }
    }
}
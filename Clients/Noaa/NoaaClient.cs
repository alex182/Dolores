using Dolores.Clients.Nasa.Models;
using Dolores.Clients.Noaa.Models;
using Dolores.Clients.Noaa.Models.Points;
using Newtonsoft.Json;

namespace Dolores.Clients.Noaa
{
    public class NoaaClient : INoaaClient
    {

        private readonly INoaaOptions _noaaOptions;
        private HttpClient _httpClient;
        private const string _userAgent = "dolores";

        public NoaaClient(INoaaOptions noaaOptions, HttpClient httpClient)
        {
            _noaaOptions = noaaOptions;
            _httpClient = httpClient;
        }

        public async Task<Dolores.Clients.Noaa.Models.Points.RelativeLocation> GetPoints(Dolores.Clients.Noaa.Models.Points.Request request)
        {
            _httpClient.DefaultRequestHeaders.Add("User-Agent", _userAgent);

            var response = await _httpClient.GetAsync($"{_noaaOptions}/points/{request.Latitude},{request.Longitude}");
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            var pointInfo = JsonConvert.DeserializeObject<RelativeLocation>(body);

            return pointInfo;
        }

        public async Task<Dolores.Clients.Noaa.Models.Forecast.Response> GetForecast(Dolores.Clients.Noaa.Models.Forecast.Request request)
        {
            _httpClient.DefaultRequestHeaders.Add("User-Agent", _userAgent);

            var response = await _httpClient.GetAsync($"{_noaaOptions}/gridpoints/EAX/{request.GridPointOne},{request.GridPointTwo}/forecast");
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            var pointInfo = JsonConvert.DeserializeObject<Dolores.Clients.Noaa.Models.Forecast.Response>(body);

            return pointInfo;
        }

    }
}

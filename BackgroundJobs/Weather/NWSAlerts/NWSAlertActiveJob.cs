using Dolores.BackgroundJobs.Weather.NWSAlerts.Models;
using Dolores.Clients.Discord.Models;
using Dolores.Clients.Models;
using Dolores.Clients.Weather.Models.Alerts.Response;

namespace Dolores.BackgroundJobs.Weather.NWSAlerts
{
    public class NWSAlertActiveJob : BaseJob
    {
        private readonly PeriodicTimer _timer = new PeriodicTimer(TimeSpan.FromMinutes(10));
        private readonly IDiscordClientOptions _discordClientOptions;
        private HttpClient _httpClient;
        private static string alertApiUrl = "https://api.weather.gov/alerts/active/zone/";

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var cache = new List<string>();
            do
            {
                var message = await GetWeatherAlerts();
                //await _utility.SendWeatherAlertNotification(message.Result);
            }
            while (await _timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested);
        }

        public NWSAlertActiveJob(HttpClient httpClient, IDiscordClientOptions discordClientOptions)
        {
            _httpClient = httpClient;
            _discordClientOptions = discordClientOptions;
        }

        internal async Task<APIResultsWrapper<ResponseBody>> GetWeatherAlerts()
        {
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            var resultList = new List<HttpResponseMessage>();
            foreach(var zoneId in AlertZoneIds.ServerZoneIDs)
            {
                resultList.Add(await _httpClient.GetAsync($"{alertApiUrl}+zoneId"));
            }
            
        }

    }
}

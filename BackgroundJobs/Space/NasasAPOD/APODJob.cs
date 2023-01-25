using Dolores.BackgroundJobs.Space.NasasAPOD.Model;
using Dolores.BackgroundJobs.Space.RocketLaunchLive.Models;
using Dolores.Clients.Discord.Models;
using Dolores.Clients.Discord.Models.DiscordWebhookMessage;
using Dolores.Clients.Nasa;
using Dolores.Clients.Nasa.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Dolores.BackgroundJobs.Space.NasasAPOD
{
    public class APODJob : BaseJob
    {
        private readonly PeriodicTimer _timer = new(TimeSpan.FromMinutes(1));
        private readonly INasaClient _nasaClient;
        private readonly APODJobOptions _apodOptions;
        private readonly IDiscordClientOptions _discordClientOptions;
        private HttpClient _httpClient;

        public APODJob(INasaClient nasaClient, APODJobOptions aPODJobOptions, IDiscordClientOptions discordClientOptions, HttpClient httpClient)
        {
            _nasaClient = nasaClient;
            _apodOptions = aPODJobOptions;
            _discordClientOptions = discordClientOptions;
            _httpClient = httpClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
                DateTime currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

                if (currentTime.TimeOfDay.Hours == _apodOptions.RunTime.Hours && currentTime.TimeOfDay.Minutes == _apodOptions.RunTime.Minutes)
                {
                    var message = await _nasaClient.GetApod();
                    await SendApod(message);
                }

            }
            while (await _timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested);

        }

        internal async Task SendApod(APODResponse apod)
        {
            if (string.IsNullOrEmpty(apod.url) || string.IsNullOrEmpty(apod.explanation) || string.IsNullOrEmpty(apod.title))
            {
                return;
            }

            var message = new DiscordWebhookMessage()
            {
                content = "Astrophotograph Of the Day 🌚"
            };

            var embed = new Embed();
            embed.title = apod.title;
            embed.description = apod.explanation;
            embed.image = new Image
            {
                url = apod.hdurl
            };

            message.embeds.Add(embed);

            using StringContent content = new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_discordClientOptions.WebhookUrl),
                Content = content
            };


            var psotresp = await _httpClient.SendAsync(request);
        }


    }
}

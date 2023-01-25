using Dolores.BackgroundJobs.Space.RocketLaunchLive.Models;
using Dolores.Clients.Discord.Models;
using Dolores.Clients.Discord.Models.DiscordWebhookMessage;
using Dolores.Clients.Models;
using Dolores.Clients.RocketLaunch;
using Dolores.Clients.RocketLaunch.Models.RocketLaunchLive.Response;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Dolores.BackgroundJobs.Space.RocketLaunchLive
{
    public class RocketLaunchLiveJob : BaseJob
    {
        private readonly PeriodicTimer _timer = new(TimeSpan.FromMinutes(1));
        private readonly RocketLaunchLiveJobOptions _rocketLaunchLiveJobOptions;
        private HttpClient _httpClient;
        private readonly IRocketLaunchLiveAPIClientOptions _rocketLaunchLiveAPIClientOptions;
        private readonly IDiscordClientOptions _discordClientOptions;

        public RocketLaunchLiveJob(RocketLaunchLiveJobOptions rocketLaunchLiveJobOptions,HttpClient httpClient,
            IRocketLaunchLiveAPIClientOptions rocketLaunchLiveAPIClientOptions, IDiscordClientOptions discordClientOptions)
        {
            _rocketLaunchLiveJobOptions = rocketLaunchLiveJobOptions;
            _httpClient = httpClient;
            _rocketLaunchLiveAPIClientOptions = rocketLaunchLiveAPIClientOptions;
            _discordClientOptions = discordClientOptions;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                do
                {
                    var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
                    DateTime currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

                    if (currentTime.TimeOfDay.Hours == _rocketLaunchLiveJobOptions.RunTime.Hours && currentTime.TimeOfDay.Minutes == _rocketLaunchLiveJobOptions.RunTime.Minutes)
                    {
                        var message = await GetLaunches(null, null);
                        await SendLaunchNotification(message.Result);
                    }

                }
                while (await _timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested);
            }
            catch (Exception ex)
            {
                Log.Error("$Failed to send Rocket Launch Notification",ex);
            }            
        }

        internal async Task<APIResultsWrapper<ResponseBody>> GetLaunches(DateTime? startDate, DateTime? endDate)
        {
            var now = startDate ?? DateTime.UtcNow;
            var tomorrow = endDate ?? now.AddDays(1);

            var afterDate = now.ToString("yyyy-MM-dd");
            var beforeDate = tomorrow.ToString("yyyy-MM-dd");

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            var result = await _httpClient.GetAsync($"{_rocketLaunchLiveAPIClientOptions.BaseUrl}/json/launches?key={_rocketLaunchLiveAPIClientOptions.ApiKey}" +
                $"&after_date={afterDate}&before_date={beforeDate}");

            if (!result.IsSuccessStatusCode)
            {
                return new APIResultsWrapper<ResponseBody>
                {
                    IsSuccessStatusCode = result.IsSuccessStatusCode,
                    StatusCode = result.StatusCode
                };
            }

            var body = await result.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<ResponseBody>(body);

            return new APIResultsWrapper<ResponseBody>
            {
                Result = response,
                IsSuccessStatusCode = result.IsSuccessStatusCode,
                StatusCode = result.StatusCode
            };
        }

        internal async Task SendLaunchNotification(ResponseBody launchInfo)
        {
            var message = new DiscordWebhookMessage()
            {
                content = "Launch Notification 🚀"
            };
            foreach (var launch in launchInfo.Result.Take(10))
            {
                var embed = new Embed();
                embed.title = launch.Name;
                embed.color = 5814783;
                embed.description = launch.Mission_Description;

                var convertedLaunchDate = DateTimeOffset.FromUnixTimeSeconds(int.Parse(launch.Sort_Date));

                embed.fields.Add(new Field
                {
                    name = "Provider",
                    value = launch.Provider.Name
                });

                embed.fields.Add(new Field
                {
                    name = "Launch Date",
                    value = convertedLaunchDate.ToString()
                });

                embed.fields.Add(new Field
                {
                    name = "Vehicle",
                    value = launch.Vehicle.Name
                });

                foreach (var mission in launch.Missions)
                {
                    var missionCount = launch.Missions.IndexOf(mission) + 1;
                    embed.fields.Add(new Field
                    {
                        name = $"Mission {missionCount}",
                        value = mission.Name
                    });
                }

                embed.fields.Add(new Field
                {
                    name = "Launch Location",
                    value = launch.Pad.Location.Name
                });

                embed.fields.Add(new Field
                {
                    name = "Description",
                    value = launch.Quicktext
                });

                message.embeds.Add(embed);
            }

            if (!message.embeds.Any())
            {
                message.content = "No launches within the next 24 hours";
            }

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

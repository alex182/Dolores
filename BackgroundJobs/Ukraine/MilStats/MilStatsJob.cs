using Dolores.BackgroundJobs.Space.NasasAPOD.Model;
using Dolores.BackgroundJobs.Space.RocketLaunchLive.Models;
using Dolores.BackgroundJobs.Ukraine.Models;
using Dolores.Clients.Discord.Models;
using Dolores.Clients.Discord.Models.DiscordWebhookMessage;
using Dolores.Clients.Nasa;
using Dolores.Clients.Nasa.Models;
using Dolores.Clients.RocketLaunch.Models.RocketLaunchLive.Response;
using Dolores.Extensions;
using Dolores.Services.UkraineStats;
using Dolores.Services.UkraineStats.Models;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Dolores.BackgroundJobs.Ukraine.MilStats
{
    public class MilStatsJob : BaseJob
    {
        private readonly PeriodicTimer _timer = new(TimeSpan.FromMinutes(1));
        private readonly Ukraine_MilStatsJobOptions _jobOptions;
        private readonly IUkraineStats_Service _ukraineStatsService;
        private HttpClient _httpClient;
        private readonly IDiscordClientOptions _discordClientOptions;

        public MilStatsJob(IDiscordClientOptions _discordClientOptions, Ukraine_MilStatsJobOptions jobOptions,
            IUkraineStats_Service ukraineStatsService, HttpClient httpClient)
        {
            _jobOptions = jobOptions;
            _discordClientOptions = _discordClientOptions;
            _ukraineStatsService = ukraineStatsService;
            _httpClient = httpClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                do
                {
                    var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
                    DateTime currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

                    if (currentTime.TimeOfDay.Hours == _jobOptions.RunTime.Hours && currentTime.TimeOfDay.Minutes == _jobOptions.RunTime.Minutes)
                    {
                        var assets = await _ukraineStatsService.GetAssetStats(currentTime);
                            var imgUrl = _ukraineStatsService.GetInfographicUrl(currentTime);
                            await SendMilStats(assets, imgUrl);
                    }

                }
                while (await _timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested);
            }
            catch(Exception ex)
            {
                Log.Error("$Failed to SendMilStats", ex);
            }
           

        }

        internal async Task SendMilStats(List<AssetStat> assets,string imgUrl)
        {
            if (!assets.Any())
            {
                return;
            }

            var message = new DiscordWebhookMessage();

            var embed = new Embed();
            embed.title = "🇺🇦 Russian Losses 🇺🇦";
            embed.image = new Image
            {
                url = imgUrl
            };

            message.embeds.Add(embed);

            foreach (var asset in assets)
            {
                embed.description += $"\n{asset.AssetCategory.GetDisplayName()} - (+{asset.DailyDiff}) {asset.Total}";
            }

            using StringContent content = new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json");

        //https://discord.com/channels/968181128504676372/1065387852189409321
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{_jobOptions.WebookUrl}?thread_id=1065387852189409321"),
                Content = content
            };


            var psotresp = await _httpClient.SendAsync(request);
        }


    }
}

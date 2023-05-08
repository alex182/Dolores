using Dolores.Clients.Discord.Models.DiscordWebhookMessage;
using Dolores.Clients.Noaa.Models.Forecast;
using DSharpPlus.Entities;
using Serilog;

namespace Dolores.Processors.Noaa
{
    public class ForecastToEmbedProcessor : IForecastToEmbedProcessor
    {
        public List<DiscordEmbed> Process(Response forecast,int takeNum = 10)
        {
            Log.Information($"Attempting to create embeds for forecast {forecast}");

            var embeds = new List<DiscordEmbed>();

            foreach (var period in forecast.properties.periods.Take(takeNum))
            {
                var embed = new DiscordEmbedBuilder()
                    .WithTitle(period.name)
                    .WithImageUrl(period.icon)
                    .WithDescription(period.detailedForecast.ToString())
                    .AddField("Start Time", period.startTime.ToString())
                    .AddField("End Time", period.endTime.ToString())
                    .AddField("Temperature", period.temperature.ToString())
                    .AddField("Chance of Precipitation", period?.probabilityOfPrecipitation?.value?.ToString() ?? "None")
                    .AddField("Relative Humidity", period.relativeHumidity.value.ToString())
                    .AddField("Wind Speed", period.windSpeed.ToString())
                    .AddField("Wind Direction", period.windDirection.ToString());
           
                embeds.Add(embed);
            }

            return embeds;
        }
    }
}

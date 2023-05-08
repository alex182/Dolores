using Dolores.Clients.Noaa.Models.Forecast;
using DSharpPlus.Entities;

namespace Dolores.Processors.Noaa
{
    public interface IForecastToEmbedProcessor
    {
        List<DiscordEmbed> Process(Response forecast, int takeNum = 10);
    }
}
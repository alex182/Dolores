using Dolores.Clients.Noaa;
using Dolores.Commands.Yarn.Models;
using Dolores.Processors.Noaa;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using HtmlAgilityPack;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Serilog;

namespace Dolores.Commands.Noaa
{

    public class NoaaCommand : ApplicationCommandModule
    {
        private INoaaClient _noaaClient;
        private IForecastToEmbedProcessor _forecastToEmbedProcessor;

        public NoaaCommand(INoaaClient noaaClient, IForecastToEmbedProcessor forecastToEmbedProcessor)
        {
            _noaaClient = noaaClient;
            _forecastToEmbedProcessor = forecastToEmbedProcessor;
        }

        [SlashCommand("forecast", "Gets the weather forecast for a given area")]
        public async Task Forecast(InteractionContext interactionContext, [Option("gridpointOne","first gridpoint")]string gridpointOne  = "44",
            [Option("gridpointTwo", "second gridpoint")] string gridpointTwo = "51")
        {
            try
            {
                Log.Information($"Attempting to get forecast for. {nameof(gridpointOne)}:{gridpointOne} {nameof(gridpointTwo)}:{gridpointTwo}");

                await interactionContext.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

                var forecast = await _noaaClient.GetForecast(
                    new Clients.Noaa.Models.Forecast.Request()
                    {
                        GridPointOne = gridpointOne,
                        GridPointTwo = gridpointTwo
                    });

                var embeds = _forecastToEmbedProcessor.Process(forecast);

                var response = new DiscordWebhookBuilder()
                    .AddEmbeds(embeds);

                await interactionContext.EditResponseAsync(response);
            }
            catch(Exception ex) 
            {
                Log.Error($"Failed to get gifs. {nameof(gridpointOne)}:{gridpointOne} {nameof(gridpointTwo)}:{gridpointTwo}",ex);
            }

        }
    }
}

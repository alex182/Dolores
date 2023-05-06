using Dolores.Clients.Noaa;
using Dolores.Commands.Yarn.Models;
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
using Newtonsoft.Json.Linq;
using Serilog;
using System.Linq;
using System.Net.Http;
using System.Xml;
using System.Xml.Linq;

namespace Dolores.Commands.Noaa
{

    public class NoaaCommand : ApplicationCommandModule
    {
        private INoaaClient _noaaClient;

        public NoaaCommand(NoaaClient noaaClient)
        {
            _noaaClient = noaaClient;
        }


        [SlashCommand("forecast", "Gets the weather forecast for a given area")]
        public async Task GetForecast(InteractionContext interactionContext, [Option("gridpointOne","first gridpoint")]string gridpointOne  = "44",
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

                var response = new DiscordWebhookBuilder()
                    .WithContent(JsonConvert.SerializeObject(forecast));

                await interactionContext.EditResponseAsync(response);
            }
            catch(Exception ex) 
            {
                Log.Error($"Failed to get gifs. {nameof(gridpointOne)}:{gridpointOne} {nameof(gridpointTwo)}:{gridpointTwo}",ex);
            }

        }
    }
}

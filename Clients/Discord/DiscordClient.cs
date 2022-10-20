using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dolores.Commands.Mocking;
using Dolores.Commands.Roulette;
using DSharpPlus.Entities;
using DSharpPlus;
using Dolores.Clients.Discord.Models;
using DSharpPlus.CommandsNext.Exceptions;
using Dolores.Commands.Sloganizer;
using Dolores.Clients.HAMqtt;
using System.Text.Json;
using Dolores.Clients.HAMqtt.Models;
using Dolores.Clients.HAMqtt.Models.RocketLaunchLive.Response;
using Newtonsoft.Json;
using Dolores.Clients.Discord.Models.DiscordWebhookMessage;

namespace Dolores.Clients.Discord
{
    public class DiscordClient : IDiscordClient
    {
        public readonly EventId BotEventId = new EventId(42, "Bot-Ex02");
        public DSharpPlus.DiscordClient _client { get; set; }
        public CommandsNextExtension _commands { get; set; }
        private readonly IMqttClient _launchMqttClient;
        private HttpClient _httpClient;
        private readonly IDiscordClientOptions _discordClientOptions;

        public DiscordClient(DSharpPlus.DiscordClient discordClient,
            CommandsNextConfiguration commandsNextConfiguration, LaunchMqttClient mqttClient, 
            HttpClient httpClient,IDiscordClientOptions discordClientOptions)
        {
            _discordClientOptions = discordClientOptions;

            _httpClient = httpClient;
            _launchMqttClient = mqttClient;

            _client = discordClient;
            _client.Ready += Client_Ready;
            _client.GuildAvailable += Client_GuildAvailable;
            _client.ClientErrored += Client_ClientError;

            _commands = _client.UseCommandsNext(commandsNextConfiguration);
            _commands.CommandExecuted += Commands_CommandExecuted;
            _commands.CommandErrored += Commands_CommandErrored;

            _commands.RegisterCommands<MockCommand>();
            _commands.RegisterCommands<SloganizerCommand>();
            //_commands.RegisterCommands<TimeoutRoulette>();
            _commands.SetHelpFormatter<MockingFormatter>();

            //wrong way to do this, but DSharp is...not great
        }

        public async Task RunBotAsync()
        {
            await _client.ConnectAsync();
            var launchClient = await _launchMqttClient.SubscribeToTopic();

            launchClient.ApplicationMessageReceivedAsync += e  =>
            {
                Console.WriteLine("Received application message.");

                string output = "";
                if (e != null)
                {
                    output = JsonConvert.SerializeObject(e);
                }

                SendLaunchNotification(output).GetAwaiter().GetResult();

                return Task.Delay(60000);
            };

            await Task.Delay(-1);
        }
        
        private async Task SendLaunchNotification(string output)
        {
            var response = JsonConvert.DeserializeObject<MqttMessage>(output);
            var data = Convert.FromBase64String(response.ApplicationMessage.Payload);
            string decodedString = Encoding.UTF8.GetString(data);

            var launchInfo = JsonConvert.DeserializeObject<MqttResponse>(decodedString);
            Console.WriteLine($"{JsonConvert.SerializeObject(launchInfo)}");

            var message = new DiscordWebhookMessage()
            {
                content = "Launch Notification 🚀"
            };
            foreach (var launch in launchInfo.Result.Result)
            {
                var embed = new Embed();
                embed.title = launch.Name;
                embed.color = 5814783;
                embed.description = launch.Mission_Description;

                var today = DateTimeOffset.Now.Date;
                var tomorrow = today.AddDays(1);
                var convertedLaunchDate = DateTimeOffset.FromUnixTimeSeconds(int.Parse(launch.Sort_Date));

                if (convertedLaunchDate.Date != tomorrow || convertedLaunchDate.Date != tomorrow)
                {
                    continue;
                }

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
                return; 

            using StringContent content = new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_discordClientOptions.WebhookUrl),
                Content = content,
            };

            request.Headers.Add("User-Agent", "PostmanRuntime/7.28.4");
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");

            var psotresp = await _httpClient.SendAsync(request);
        }

        private Task Client_Ready(DSharpPlus.DiscordClient sender, ReadyEventArgs e)
        {
            // let's log the fact that this event occured
            sender.Logger.LogInformation(BotEventId, "Client is ready to process events.");

            // since this method is not async, let's return
            // a completed task, so that no additional work
            // is done
            return Task.CompletedTask;
        }

        private Task Client_GuildAvailable(DSharpPlus.DiscordClient sender, GuildCreateEventArgs e)
        {
            // let's log the name of the guild that was just
            // sent to our client
            sender.Logger.LogInformation(BotEventId, $"Guild available: {e.Guild.Name}");

            // since this method is not async, let's return
            // a completed task, so that no additional work
            // is done
            return Task.CompletedTask;
        }

        private Task Client_ClientError(DSharpPlus.DiscordClient sender, ClientErrorEventArgs e)
        {
            // let's log the details of the error that just 
            // occured in our client
            sender.Logger.LogError(BotEventId, e.Exception, "Exception occured");

            // since this method is not async, let's return
            // a completed task, so that no additional work
            // is done
            return Task.CompletedTask;
        }

        private Task Commands_CommandExecuted(CommandsNextExtension sender, CommandExecutionEventArgs e)
        {
            // let's log the name of the command and user
            e.Context.Client.Logger.LogInformation(BotEventId, $"{e.Context.User.Username} successfully executed '{e.Command.QualifiedName}'");

            // since this method is not async, let's return
            // a completed task, so that no additional work
            // is done
            return Task.CompletedTask;
        }

        private async Task Commands_CommandErrored(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
            // let's log the error details
            e.Context.Client.Logger.LogError(BotEventId, $"{e.Context.User.Username} tried executing '{e.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {e.Exception.GetType()}: {e.Exception.Message ?? "<no message>"}", DateTime.Now);

            // let's check if the error is a result of lack
            // of required permissions
            if (e.Exception is ChecksFailedException ex)
            {
                // yes, the user lacks required permissions, 
                // let them know

                var emoji = DiscordEmoji.FromName(e.Context.Client, ":no_entry:");

                // let's wrap the response into an embed
                var embed = new DiscordEmbedBuilder
                {
                    Title = "Access denied",
                    Description = $"{emoji} You do not have the permissions required to execute this command.",
                    Color = new DiscordColor(0xFF0000) // red
                };
                await e.Context.RespondAsync(embed);
            }
        }
    }
}

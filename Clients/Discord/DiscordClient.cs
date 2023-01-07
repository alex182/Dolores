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
using System.Text.Json;
using Dolores.Clients.RocketLaunch.Models.RocketLaunchLive.Response;
using Newtonsoft.Json;
using Dolores.Clients.Discord.Models.DiscordWebhookMessage;
using Dolores.Commands.NytSpeaker;
using Dolores.Commands.Space;

namespace Dolores.Clients.Discord
{
    public class DiscordClient : IDiscordClient
    {
        public readonly EventId BotEventId = new EventId(42, "Bot-Ex02");
        public DSharpPlus.DiscordClient _client { get; set; }
        public CommandsNextExtension _commands { get; set; }
        private HttpClient _httpClient;
        private readonly IDiscordClientOptions _discordClientOptions;

        public DiscordClient(DSharpPlus.DiscordClient discordClient,
            CommandsNextConfiguration commandsNextConfiguration,
            HttpClient httpClient,IDiscordClientOptions discordClientOptions)
        {
            _discordClientOptions = discordClientOptions;

            _httpClient = httpClient;

            _client = discordClient;
            _client.Ready += Client_Ready;
            _client.GuildAvailable += Client_GuildAvailable;
            _client.ClientErrored += Client_ClientError;

            _commands = _client.UseCommandsNext(commandsNextConfiguration);
            _commands.CommandExecuted += Commands_CommandExecuted;
            _commands.CommandErrored += Commands_CommandErrored;

            _commands.RegisterCommands<MockCommand>();
            _commands.RegisterCommands<SloganizerCommand>();
            _commands.RegisterCommands<NytSpeakerCommand>();
            //_commands.RegisterCommands<SpaceCommand>();
            //_commands.RegisterCommands<TimeoutRoulette>();
            _commands.SetHelpFormatter<MockingFormatter>();

            //wrong way to do this, but DSharp is...not great
        }

        public async Task RunBotAsync()
        {
            await _client.ConnectAsync();
            await Task.Delay(-1);
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

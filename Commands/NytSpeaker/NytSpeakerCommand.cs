using Dolores.Clients.Discord.Models.DiscordWebhookMessage;
using Dolores.Commands.NytSpeaker.Model;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Dolores.Commands.NytSpeaker
{
    public class NytSpeakerCommand : BaseCommandModule
    {
        private static bool isRunning = false;
        private readonly IUtility _utility;

        public NytSpeakerCommand(IUtility utility)
        {
            _utility = utility;
        }

        [Command("getCurrentVote")]
        [Description("Gets the current Speaker Of The House Vote")]
        public async Task GetCurrentVote(CommandContext ctx)
        {
            if (!isRunning)
            {
                isRunning = true;
                await GetAndPrintLatestVotes(ctx);
            }
        }

        private async Task GetAndPrintLatestVotes(CommandContext ctx)
        {
            float lastVotes = 0;
            DiscordMessage discordMessage = null;
            do
            {
                await ctx.TriggerTypingAsync();

                var votesJson = await _utility.GetVote();
                var deserialized = JsonConvert.DeserializeObject<List<NytSpeakerResponse>>(votesJson);
                NytSpeakerResponse? mostRecent = deserialized?.Last();
                if (mostRecent == null)
                    break;

                float totalVotes = 0;
                var nominees = new List<VoteCount>();

                var embedBuilder = new DiscordEmbedBuilder();
                embedBuilder.Title = $"Round: {mostRecent.vote_round}";

                foreach (var nominee in mostRecent.values)
                {
                    var nomineeToAdd = new VoteCount();
                    nomineeToAdd.NomineeName = nominee.key;
                    nomineeToAdd.TotalVotes = nominee.total;

                    if (nominee.key.ToLower() != "present")
                        totalVotes += nominee.total;

                    nominees.Add(nomineeToAdd);
                }

                if (lastVotes == totalVotes) break;

                foreach (var nominee in nominees)
                {
                    var percentOfVote = "0";
                    if (nominee.NomineeName.ToLower() != "present")
                    {
                        percentOfVote = string.Format("{0:P2}", nominee.TotalVotes / totalVotes);
                    }
                    var messageToSend = $"{nominee.TotalVotes} - {percentOfVote}";
                    embedBuilder.AddField($"{nominee.NomineeName}", messageToSend, true);
                }
                if (discordMessage == null ||( discordMessage.Channel.LastMessageId != discordMessage.Id &&  DateTimeOffset.Now.Subtract(discordMessage.CreationTimestamp) > new TimeSpan(0, 5, 0)))
                    discordMessage = await ctx.RespondAsync(embedBuilder.Build());
                else
                    discordMessage = await discordMessage.ModifyAsync(embedBuilder.Build());
                lastVotes = totalVotes;

                Thread.Sleep(new TimeSpan(0, 1, 0));

            } while (lastVotes > 0);

            isRunning= false;
        }
    }
}

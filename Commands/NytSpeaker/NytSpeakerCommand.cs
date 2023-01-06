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
        private readonly IUtility _utility;

        public NytSpeakerCommand(IUtility utility)
        {
            _utility = utility;
        }

        [Command("getCurrentVote")]
        [Description("Gets the current Speaker Of The House Vote")]
        public async Task GetCurrentVote(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            var message = await _utility.GetVote();
            var deserialized = JsonConvert.DeserializeObject<List<NytSpeakerResponse>>(message);
            var mostRecent = deserialized[deserialized.Count - 1];

            float totalVotes = 0;
            var nominees = new List<VoteCount>();

            var embedBuilder = new DiscordEmbedBuilder();
            embedBuilder.Title = $"Round: {mostRecent.vote_round}";

            foreach (var nominee in mostRecent.values)
            {
                var nomineeToAdd = new VoteCount();
                nomineeToAdd.NomineeName = nominee.key;
                nomineeToAdd.TotalVotes = nominee.total;
                totalVotes += nominee.total;

                nominees.Add(nomineeToAdd);
            }

            foreach (var nominee in nominees)
            {
                float percentOfVote = nominee.TotalVotes/totalVotes;
                var messageToSend = $"{nominee.TotalVotes} - {string.Format("{0:P2}", percentOfVote)}";
                embedBuilder.AddField($"{nominee.NomineeName}", messageToSend, true);
            }
            await ctx.RespondAsync(embedBuilder.Build());

        }
    }
}

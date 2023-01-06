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

            var messageToSend = $"Round: {mostRecent.vote_round}";
            await ctx.RespondAsync(messageToSend);

            foreach (var nominee in mostRecent.values)
            {
                var name = nominee.key;
                var republicanVotes = 0;
                var democratVotes = 0;
                var otherVotes = 0;

                foreach (var votes in nominee.values)
                {
                    messageToSend = "";

                    if (votes.key.ToLower() == "d")
                    {
                        democratVotes = votes.value;
                    }
                    else if (votes.key.ToLower() == "r")
                    {
                        republicanVotes = votes.value;
                    }
                    else
                    {
                        otherVotes += votes.value;
                    }

                    messageToSend = $" Nominee: {name} Republican Votes: {republicanVotes} Democrat Votes: {democratVotes} Other: {otherVotes} Total: {nominee.total}";
                    await ctx.RespondAsync(messageToSend);
                }
            }

        }
    }
}

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolores.Commands.Roulette
{
    public class TimeoutRoulette : BaseCommandModule
    {
        private readonly Utility _utility;

        public TimeoutRoulette()
        {
            _utility = new Utility();
        }


        [Command("timeout")] // let's define this method as a command
        [Description("Timeout Roulette Bitches!")] // this will be displayed to tell users what this command does when they invoke help
        [Aliases("spicy")] // alternative names for the command
        public async Task Timeout(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            var channelExecutedIn = ctx.Channel;

            var guild = ctx.Guild;
            var members = await guild.GetAllMembersAsync();

            var rand = new Random();
            var timedOutUserIndex = rand.Next(0, members.Count - 1);
            DiscordMember userForTimeout;
            if (members != null)
            {
                var listOfMembers = members.ToList();
                userForTimeout = listOfMembers[timedOutUserIndex];
            }
            else
            {
                userForTimeout = ctx.Member;
            }

            var mockTheTimeout = new Mocking.MockCommand();
            if (channelExecutedIn.ToString().Contains("#general"))
            {
                await ctx.RespondAsync($"{ctx.Guild.EveryoneRole} LET'S PLAY SOME TIMEOUT ROULETTE MOTHER FUCKERS!");
                Thread.Sleep(1000);
                await mockTheTimeout.Insult(ctx, userForTimeout);
                await userForTimeout.TimeoutAsync(new DateTimeOffset(DateTime.Now.AddMinutes(1)));
            }
            else
            {
                await ctx.RespondAsync($"{ctx.Member.Mention} We only play timeout roulette in the general channel so EVERYONE can play <3");
            }
        }
    }
}

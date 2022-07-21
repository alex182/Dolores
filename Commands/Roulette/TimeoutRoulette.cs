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
            var usersInChannel = channelExecutedIn.Users;

            var rand = new Random();
            var timedOutUserIndex = rand.Next(0, usersInChannel.Count - 1);
            var userForTimeout = usersInChannel[timedOutUserIndex];

            var mockTheTimeout = new Mocking.MockCommand();
            mockTheTimeout.Insult(ctx, userForTimeout);
            userForTimeout.TimeoutAsync(new DateTimeOffset(DateTime.Now.AddMinutes(1)));
        }
    }
}

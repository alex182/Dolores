using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolores.Commands.Space
{
    public class SpaceCommand : BaseCommandModule
    {
        private readonly IUtility _utility;

        public SpaceCommand(IUtility utility)
        {
            _utility = utility;
        }

        [Command("launchestoday")]
        [Description("Gets info about launches that are happening today. Can only send info aboutthe first 10 launches.")]
        public async Task LaunchesToday(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            var message = await _utility.GetLaunches();

            await _utility.SendLaunchNotification(message.Result);
        }
    }
}

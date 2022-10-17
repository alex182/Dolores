using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolores.Commands.Sloganizer
{
    public class SloganizerCommand : BaseCommandModule
    {
        private readonly IUtility _utility;


        public SloganizerCommand(IUtility utility)
        {
            _utility = utility;
        }


        [Command("sloganize")] 
        [Description("creates a random slogan")]
        public async Task Mock(CommandContext ctx,string sloganWord)
        {
            await ctx.TriggerTypingAsync();

            var message = await _utility.GetSlogan(sloganWord);

            await ctx.RespondAsync(message);
        }
    }
}

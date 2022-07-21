using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolores.Commands.Mocking
{
    public class MockCommand : BaseCommandModule
    {
        private readonly Utility _utility;
        private MemeGenerator _memeGenerator;

        public MockCommand()
        {
            _utility = new Utility();
            _memeGenerator = new MemeGenerator();
        }

        [Command("ping")] // let's define this method as a command
        [Description("Example ping command")] // this will be displayed to tell users what this command does when they invoke help
        [Aliases("pong")] // alternative names for the command
        public async Task Ping(CommandContext ctx) // this command takes no arguments
        {
            // let's trigger a typing indicator to let
            // users know we're working
            await ctx.TriggerTypingAsync();

            // let's make the message a bit more colourful
            var emoji = DiscordEmoji.FromName(ctx.Client, ":ping_pong:");

            // respond with ping
            await ctx.RespondAsync($"{emoji} Pong! Ping: {ctx.Client.Ping}ms");
        }

        [Command("mock")] // let's define this method as a command
        [Description("Mocks the tagged person")] // this will be displayed to tell users what this command does when they invoke help
        [Aliases("youSuck")] // alternative names for the command
        public async Task Mock(CommandContext ctx, DiscordMember member)
        {
            await ctx.TriggerTypingAsync();
            var message = await _utility.GetLastMessageAsync(ctx, member);

            var sarcasticMessage = string.Empty;

            if (message == null)
            {
                await ctx.RespondAsync($"Couldn't find a message for {member.Mention}");
            }

            var sarcasticImage = _memeGenerator.CreateSpongeBob(_utility.Sarcastify(message));
            var messageBuilder = new DiscordMessageBuilder();

            using(FileStream fs = File.OpenRead(sarcasticImage))
            {
                var messageToSend = messageBuilder
                .WithFile(sarcasticImage, fs);

                await ctx.RespondAsync(messageToSend);
            }
        }
    }
}

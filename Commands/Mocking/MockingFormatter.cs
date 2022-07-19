﻿using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolores.Commands.Mocking
{
    public class MockingFormatter : BaseHelpFormatter
    {
        private StringBuilder MessageBuilder { get; }

        public MockingFormatter(CommandContext ctx) : base(ctx)
        {
            this.MessageBuilder = new StringBuilder();
        }

        // this method is called first, it sets the command
        public override BaseHelpFormatter WithCommand(Command command)
        {
            this.MessageBuilder.Append("Command: ")
               .AppendLine(Formatter.Bold(command.Name))
               .AppendLine();


            this.MessageBuilder.Append("Description: ")
                .AppendLine(command.Description)
                .AppendLine();

            if (command is CommandGroup)
                this.MessageBuilder.AppendLine("This group has a standalone command.").AppendLine();

            this.MessageBuilder.Append("Aliases: ")
                .AppendLine(string.Join(", ", command.Aliases))
                .AppendLine();


            foreach (var overload in command.Overloads)
            {
                if (overload.Arguments.Count == 0)
                {
                    continue;
                }

                this.MessageBuilder.Append($"[Overload {overload.Priority}] Arguments: ")
                .AppendLine(string.Join(", ", overload.Arguments.Select(xarg => $"{xarg.Name} ({xarg.Type.Name})")))
                .AppendLine();
            }

            return this;
        }

        // this method is called second, it sets the current group's subcommands
        // if no group is being processed or current command is not a group, it 
        // won't be called
        public override BaseHelpFormatter WithSubcommands(IEnumerable<Command> subcommands)
        {
            this.MessageBuilder.Append("Subcommands: ")
                .AppendLine(string.Join(", ", subcommands.Select(xc => xc.Name)))
                .AppendLine();

            return this;
        }

        // this is called as the last method, this should produce the final 
        // message, and return it
        public override CommandHelpMessage Build()
        {
            return new CommandHelpMessage(this.MessageBuilder.ToString().Replace("\r\n", "\n"));
        }
    }
}

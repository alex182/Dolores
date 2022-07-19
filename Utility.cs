using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolores
{
    public class Utility
    {
        public async Task<string> GetLastMessageAsync(CommandContext ctx, DiscordMember member)
        {
            var messages = await ctx.Channel.GetMessagesAsync(100);
            messages = messages.ToList();
            return messages.Where(message => message.Author.Id.Equals(member.Id))?.FirstOrDefault(message => message.Content != ".")?.Content;
        }

        public string Sarcastify(string word)
        {
            word = word.ToLower();
            var newWord = "";
            for (var i = 0; i < word.Length; i++)
            {
                var letter = word[i];
                if (i % 2 == 0)
                {
                    letter = Char.ToUpper(letter);
                }

                newWord += letter;
            }

            newWord += " ";

            return newWord;
        }

    }
}

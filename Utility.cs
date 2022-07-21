using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
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

        public string RandomInsult(string name)
        {
            string insult = "";
            

            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri("https://evilinsult.com/");
                client.DefaultRequestHeaders.Accept.Clear();
               

                var insultFromApi = GetInsult(client, name);
            }
            catch(Exception e)
            {
                Console.WriteLine($"Exception: {e.Message}");
                return $"Api must be down... But HEY {name}, FUCK YOU BUDDY.";
            }

            return insult;
        }

        private async Task GetInsult(HttpClient client, string name)
        {
            var response = await client.GetAsync("generate_insult.php?type=xml&lang=en");
            response.EnsureSuccessStatusCode();

            var content = response.Content.ReadAsStringAsync();

            var otherShit = response.Content.ReadAsStringAsync();
            var insultFromSplit = content.ToString().Split('>')[6];

            insultFromApi = insultFromSplit;
        }
    }
}

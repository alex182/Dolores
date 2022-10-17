using Dolores.Models.InsultApi;
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
    public class Utility : IUtility
    {
        private readonly HttpClient _httpClient;

        public Utility(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

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

        public async Task<string> RandomInsult(string name)
        {
            string insult = "";

            try
            {
                var insultFromApi = await GetInsult(name);
                insult = insultFromApi.insult;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e.Message}");
                return $"Api must be down... But HEY {name}, FUCK YOU BUDDY.";
            }

            return insult;
        }

        private async Task<InsultApiResponse> GetInsult(string name)
        {
            _httpClient.BaseAddress = new Uri("https://evilinsult.com/");
            _httpClient.DefaultRequestHeaders.Accept.Clear();

            var rand = new Random();
            var nameRand = name + rand.Next(9999);
            var response = await _httpClient.GetAsync($"generate_insult.php?lang=en&type=json&name={nameRand}");
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            body = body.Replace("&amp;", "&");

            return JsonConvert.DeserializeObject<InsultApiResponse>(body);
        }
    }
}
